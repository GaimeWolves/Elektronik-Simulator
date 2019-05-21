using OESim.Circuit.Logic.Components;
using OESim.Circuit.Logic.Components.Linear;
using OESim.Circuit.Visualization;
using OESim.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OESim
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Wire? _CurrentWire;
        Component _CurrentComponent;
        Point _InitialClick;
        Point _LastPosition;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _CircuitView.Invalidate(true, true);
            _Components.Children.Add(UIUtils.CreateComponentButton(new Component(new Resistor()), _Components.ActualWidth, OnComponentButtonPressed));
        }

        private void OnComponentButtonPressed(object sender, RoutedEventArgs e)
        {
            if (_CurrentWire.HasValue)
            {
                _CircuitView.DeleteWire(_CurrentWire.Value);
                _CurrentWire = null;
            }
            Grid grd = (sender as Button).Content as Grid;
            Viewbox cView = grd.Children[0] as Viewbox;
            Component c = cView.Child as Component;
            IEComponent iC = c.IComponent;
            _CurrentComponent = new Component(Activator.CreateInstance(iC.GetType()) as IEComponent);
            _CircuitView.AddComponent(ref _CurrentComponent, Mouse.GetPosition(_CircuitView));
        }

        private void OnCircuitLeftClicked(object sender, MouseButtonEventArgs e)
        {
            if (_CurrentWire.HasValue)
                return;
            _InitialClick = Mouse.GetPosition(_CircuitView);

            if (_CurrentComponent is null)
                _CurrentWire = _CircuitView.CreateWire(_InitialClick, _InitialClick);
            else
                _CurrentComponent = null;
        }

        private void OnCircuitLeftReleased(object sender, MouseButtonEventArgs e)
        {
            if (_CurrentWire.HasValue)
            {
                if (_CurrentWire.Value.P1 == _CurrentWire.Value.P2)
                    _CircuitView.DeleteWire(_CurrentWire.Value);
                _CurrentWire = null;
            }
        }

        private void OnCircuitRightClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(_CurrentComponent is null))
                _CircuitView.UpdateComponent(ref _CurrentComponent, _CurrentComponent.Position, _CurrentComponent.Angle + 90);
        }

        private void OnCircuitMouseMoved(object sender, MouseEventArgs e)
        {
            if (_CurrentWire.HasValue)
                _CurrentWire = _CircuitView.UpdateWire(_CurrentWire.Value, _InitialClick, Mouse.GetPosition(_CircuitView));
            else if (!(_CurrentComponent is null))
                _CircuitView.UpdateComponent(ref _CurrentComponent, Mouse.GetPosition(_CircuitView), _CurrentComponent.Angle);

            if (e.RightButton == MouseButtonState.Pressed)
                _CircuitView.Offset += Mouse.GetPosition(_CircuitView) - _LastPosition;

            _LastPosition = Mouse.GetPosition(_CircuitView);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        _CircuitView.Zoom++;
                    else
                        _CircuitView.Move(new Point(0, _CircuitView.Zoom / 2));
                    break;
                case Key.Down:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        _CircuitView.Zoom--;
                    else
                        _CircuitView.Move(new Point(0, -_CircuitView.Zoom / 2));
                    break;
                case Key.Left:
                    _CircuitView.Move(new Point(_CircuitView.Zoom / 2, 0));
                    break;
                case Key.Right:
                    _CircuitView.Move(new Point(-_CircuitView.Zoom / 2, 0));
                    break;
                case Key.Back:
                    _CircuitView.DeleteKeyPressed();
                    break;
                case Key.Delete:
                    _CircuitView.DeleteKeyPressed();
                    break;
                case Key.R:
                    if (!(_CurrentComponent is null))
                        _CircuitView.UpdateComponent(ref _CurrentComponent, Mouse.GetPosition(_CircuitView), _CurrentComponent.Angle + 90);
                    break;
            }
        }

        private void OnCircuitScrolled(object sender, MouseWheelEventArgs e)
        {
            _CircuitView.Zoom += e.Delta;
        }

        private void OnCircuitSizeChanged(object sender, SizeChangedEventArgs e) => _CircuitView.Invalidate(false, true);
    }
}
