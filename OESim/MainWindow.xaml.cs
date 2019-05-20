﻿using OESim.Circuit.Logic.Components.Linear;
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
            Resistor r = new Resistor();
            Component c = new Component(r, r.CreateVisual());
            _Components.Children.Add(UIUtils.CreateComponentButton(c, "Resistor", 100));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        private void OnCircuitLeftClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(_CurrentWire is null))
                return;
            _InitialClick = Mouse.GetPosition(_CircuitView);
            _CurrentWire = _CircuitView.CreateWire(_InitialClick, _InitialClick);
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

        private void OnCircuitMouseMoved(object sender, MouseEventArgs e)
        {
            if (!(_CurrentWire is null))
                _CurrentWire = _CircuitView.UpdateWire(_CurrentWire.Value, _InitialClick, Mouse.GetPosition(_CircuitView)); 

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
            }
        }

        private void OnCircuitScrolled(object sender, MouseWheelEventArgs e)
        {
            _CircuitView.Zoom += e.Delta;
        }

        private void OnCircuitSizeChanged(object sender, SizeChangedEventArgs e) => _CircuitView.Invalidate();
    }
}
