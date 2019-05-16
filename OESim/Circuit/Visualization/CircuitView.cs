using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OESim.Circuit.Visualization
{
    /// <summary>
    /// Führen Sie die Schritte 1a oder 1b und anschließend Schritt 2 aus, um dieses benutzerdefinierte Steuerelement in einer XAML-Datei zu verwenden.
    ///
    /// Schritt 1a) Verwenden des benutzerdefinierten Steuerelements in einer XAML-Datei, die im aktuellen Projekt vorhanden ist.
    /// Fügen Sie dieses XmlNamespace-Attribut dem Stammelement der Markupdatei 
    /// an der Stelle hinzu, an der es verwendet werden soll:
    ///
    ///     xmlns:MyNamespace="clr-namespace:OESim.Circuit.Visualization"
    ///
    ///
    /// Schritt 1b) Verwenden des benutzerdefinierten Steuerelements in einer XAML-Datei, die in einem anderen Projekt vorhanden ist.
    /// Fügen Sie dieses XmlNamespace-Attribut dem Stammelement der Markupdatei 
    /// an der Stelle hinzu, an der es verwendet werden soll:
    ///
    ///     xmlns:MyNamespace="clr-namespace:OESim.Circuit.Visualization;assembly=OESim.Circuit.Visualization"
    ///
    /// Darüber hinaus müssen Sie von dem Projekt, das die XAML-Datei enthält, einen Projektverweis
    /// zu diesem Projekt hinzufügen und das Projekt neu erstellen, um Kompilierungsfehler zu vermeiden:
    ///
    ///     Klicken Sie im Projektmappen-Explorer mit der rechten Maustaste auf das Zielprojekt und anschließend auf
    ///     "Verweis hinzufügen"->"Projekte"->[Navigieren Sie zu diesem Projekt, und wählen Sie es aus.]
    ///
    ///
    /// Schritt 2)
    /// Fahren Sie fort, und verwenden Sie das Steuerelement in der XAML-Datei.
    ///
    ///     <MyNamespace:CircuitView/>
    ///
    /// </summary>
    public class CircuitView : Canvas
    {
        private static readonly double NormalZoom = 25, MaxZoom = 100, MinZoom = 5;

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Point), typeof(CircuitView), new FrameworkPropertyMetadata(new Point()));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(CircuitView), new FrameworkPropertyMetadata(NormalZoom));

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
        private TransformGroup _Transform;

        static CircuitView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircuitView), new FrameworkPropertyMetadata(typeof(CircuitView)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _Wires = new List<Wire>();
            _Transform = new TransformGroup();            
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
                e.RenderTransform = _Transform;
        }

        public void Invalidate() => UpdateImage();

        public void Move(Point dP)
        {
            Offset = new Point(Offset.X + dP.X, Offset.Y + dP.Y);
        }

        public Wire CreateWire(Point p1, Point p2)
        {
            Wire wire = new Wire(GetNearestIndex(p1), GetNearestIndex(p2), _Transform);
            _Wires.Add(wire);
            Children.Add(wire.Path);
            return wire;
        }

        public void UpdateWire(Wire wire, Point p1, Point p2)
        {
            wire.P1 = GetNearestIndex(p1);
            wire.P2 = GetNearestIndex(p2);
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
