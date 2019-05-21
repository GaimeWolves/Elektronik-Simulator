using OESim.Circuit.Logic.Components.Linear;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OESim.Circuit.Visualization
{
    public class CircuitView : Canvas
    {
        private static readonly double NormalZoom = 25, MaxZoom = 100, MinZoom = 5;

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Point), typeof(CircuitView), new FrameworkPropertyMetadata(new Point()));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(CircuitView), new FrameworkPropertyMetadata(NormalZoom));

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty); 
            set { SetValue(OffsetProperty, value); UpdateImage(true, true); }
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set { SetValue(ZoomProperty, Math.Min(MaxZoom, Math.Max(MinZoom, value))); UpdateImage(true, true); }
        }

        private Wire? _SelectedWire;
        private Component _SelectedComponent;

        private List<Wire> _Wires;
        private List<Component> _Components;
        private TransformGroup _Transform;

        static CircuitView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircuitView), new FrameworkPropertyMetadata(typeof(CircuitView)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _Wires = new List<Wire>();
            _Components = new List<Component>();
            _Transform = new TransformGroup();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (_SelectedWire.HasValue)
            {
                if (!_SelectedWire.Value.Path.IsMouseOver)
                {
                    _SelectedWire.Value.Path.Stroke = Brushes.Black;
                    _SelectedWire = null;
                }
            }
            else if (!(_SelectedComponent is null))
            {
                if (!_SelectedComponent.IsMouseOver)
                {
                    foreach (UIElement element in _SelectedComponent.Children)
                    {
                        if (element is Path)
                            (element as Path).Stroke = Brushes.Black;
                    }
                    _SelectedComponent = null;
                }
            }

            e.Handled = false;
            base.OnMouseDown(e);
        }

        private void UpdateImage(bool circuitInvalid, bool gridInvalid)
        {
            if (gridInvalid)
            {
                int width = (int)ActualWidth, height = (int)ActualHeight;
                if (width == 0) return;
                byte[] img = new byte[width * height * 4];
                for (double x = Offset.X % Zoom; x < width; x += Zoom)
                    for (double y = Offset.Y % Zoom; y < height; y += Zoom)
                        if (x > 0 && y > 0)
                            img[(int)y * width * 4 + (int)x * 4 + 3] = 255;

                Background = new ImageBrush(BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, img, width * 4));
            }

            if (circuitInvalid)
            {
                _Transform.Children.Clear();
                _Transform.Children.Add(new ScaleTransform(Zoom, Zoom));
                _Transform.Children.Add(new TranslateTransform(Offset.X, Offset.Y));
                foreach (UIElement e in Children)
                {
                    e.RenderTransform = _Transform;
                    if (e is Component)
                    {
                        TransformGroup newTransform = new TransformGroup();
                        newTransform.Children.Add(new RotateTransform((e as Component).Angle));
                        newTransform.Children.Add(new ScaleTransform(Zoom, Zoom));
                        newTransform.Children.Add(new TranslateTransform(Offset.X + (e as Component).Position.X * Zoom, Offset.Y + (e as Component).Position.Y * Zoom));
                        e.RenderTransform = newTransform;
                    }
                }
            }
        }

        public void Invalidate(bool circuitInvalid, bool gridInvalid) => UpdateImage(circuitInvalid, gridInvalid);

        public void Move(Point dP)
        {
            Offset = new Point(Offset.X + dP.X, Offset.Y + dP.Y);
        }

        public void DeleteKeyPressed()
        {
            if (_SelectedWire.HasValue)
                DeleteWire(_SelectedWire.Value);
            if (!(_SelectedComponent is null))
                DeleteComponent(_SelectedComponent);
        }

        public void DeleteWire(Wire wire)
        {
            if (_SelectedWire.HasValue)
                if (_SelectedWire.Value.Equals(wire))
                    _SelectedWire = null;
            Children.Remove(wire.Path);
            _Wires.Remove(wire);
        }

        public void DeleteComponent(Component c)
        {
            if (!(_SelectedComponent is null))
                if (_SelectedComponent.Equals(c))
                    _SelectedComponent = null;
            Children.Remove(c);
            _Components.Remove(c);
        }

        public Wire CreateWire(Point p1, Point p2)
        {
            Wire wire = new Wire(GetNearestIndex(p1), GetNearestIndex(p2), _Transform);
            wire.Path.MouseDown += (s, args) =>
            {
                if (!(_SelectedComponent is null))
                {
                    foreach (UIElement e in _SelectedComponent.Children)
                    {
                        if (e is Path)
                            (e as Path).Stroke = Brushes.Black;
                    }
                    _SelectedComponent = null;
                }

                if (_SelectedWire.HasValue)
                    _SelectedWire.Value.Path.Stroke = Brushes.Black;
                _SelectedWire = wire;
                _SelectedWire.Value.Path.Stroke = Brushes.Blue;
            };
            _Wires.Add(wire);
            Children.Add(wire.Path);
            return wire;
        }

        public Wire UpdateWire(Wire wire, Point p1, Point p2)
        {
            wire.P1 = GetNearestIndex(p1);
            wire.P2 = GetNearestIndex(p2);
            return wire;
        }

        public void AddComponent(ref Component c, Point p)
        {
            _Components.Add(c);
            int index = _Components.IndexOf(c);
            Children.Add(c);
            c.MouseLeftButtonDown += (s, args) =>
            {
                if (_SelectedWire.HasValue)
                {
                    _SelectedWire.Value.Path.Stroke = Brushes.Black;
                    _SelectedWire = null;
                }

                if (!(_SelectedComponent is null))
                {
                    foreach (UIElement e in _SelectedComponent.Children)
                    {
                        if (e is Path)
                            (e as Path).Stroke = Brushes.Black;
                    }
                }
                _SelectedComponent = _Components[index];
                foreach (UIElement e in _SelectedComponent.Children)
                {
                    if (e is Path)
                        (e as Path).Stroke = Brushes.Blue;
                }
            };
            UpdateComponent(ref c, p, 0);
        }

        public void UpdateComponent(ref Component c, Point p, double angle)
        {
            c.Position = GetNearestIndex(p);
            c.Angle = angle % 360;
            Invalidate(true, false);
        }

        private Point GetNearestIndex(Point pos)
        {
            return new Point
            {
                X = Math.Round((pos.X - Offset.X) / Zoom),
                Y = Math.Round((pos.Y - Offset.Y) / Zoom)
            };
        }
    }
}
