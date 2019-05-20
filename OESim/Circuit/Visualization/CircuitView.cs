using OESim.Circuit.Logic.Components.Linear;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OESim.Circuit.Visualization
{
    public class CircuitView : Canvas
    {
        private static readonly double NormalZoom = 25, MaxZoom = 100, MinZoom = 5;

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Point), typeof(CircuitView), new FrameworkPropertyMetadata(new Point()));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(CircuitView), new FrameworkPropertyMetadata(NormalZoom));

        private Wire? _Selected;

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty); 
            set { SetValue(OffsetProperty, value); UpdateImage(); }
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set { SetValue(ZoomProperty, Math.Min(MaxZoom, Math.Max(MinZoom, value))); UpdateImage(); }
        }

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
            if (_Selected.HasValue)
            {
                if (!_Selected.Value.Path.IsMouseOver)
                {
                    _Selected.Value.Path.Stroke = Brushes.Black;
                    _Selected = null;
                }
            }

            e.Handled = false;
            base.OnMouseDown(e);
        }

        private void UpdateImage()
        {
            int width = (int)ActualWidth, height = (int)ActualHeight;
            if (width == 0) return;
            byte[] img = new byte[width * height * 4];
            for (double x = Offset.X % Zoom; x < width; x += Zoom)
                for (double y = Offset.Y % Zoom; y < height; y += Zoom)
                    if (x > 0 && y > 0)
                        img[(int)y * width * 4 + (int)x * 4 + 3] = 255;

            Background = new ImageBrush(BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, img, width * 4));

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

        public void Invalidate() => UpdateImage();

        public void Move(Point dP)
        {
            Offset = new Point(Offset.X + dP.X, Offset.Y + dP.Y);
        }

        public void DeleteKeyPressed()
        {
            if (_Selected.HasValue)
            {
                Children.Remove(_Selected.Value.Path);
                _Wires.Remove(_Selected.Value);
                _Selected = null;
            }
        }

        public void DeleteWire(Wire wire)
        {
            if (_Selected.HasValue)
                if (_Selected.Value.Equals(wire))
                    _Selected = null;
            Children.Remove(wire.Path);
            _Wires.Remove(wire);
        }

        public Wire CreateWire(Point p1, Point p2)
        {
            Wire wire = new Wire(GetNearestIndex(p1), GetNearestIndex(p2), _Transform);
            wire.Path.MouseDown += (s, e) =>
            {
                if (_Selected.HasValue)
                    _Selected.Value.Path.Stroke = Brushes.Black;
                _Selected = wire;
                _Selected.Value.Path.Stroke = Brushes.Blue;
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
