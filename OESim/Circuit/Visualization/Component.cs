using OESim.Circuit.Logic.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace OESim.Circuit.Visualization
{
    public class Component : Canvas
    {
        public IEComponent IComponent { get; private set; }
        private string _Name;

        public Point Position { get; set; }
        public double Angle { get; set; }

        static Component()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Component), new FrameworkPropertyMetadata(typeof(Component)));
        }

        public Component() : base()
        {
            
        }

        public Component(IEComponent component) : this()
        {
            IComponent = component;
            UIElement cVisual = component.CreateVisual();

            Children.Add(cVisual);

            double boundsX = IComponent.GetAttribute<double>("BoundsX");
            double boundsY = IComponent.GetAttribute<double>("BoundsY");

            _Name = IComponent.GetAttribute<string>("Name");

            Canvas.SetTop(cVisual, -boundsY / 2);
            Canvas.SetLeft(cVisual, -boundsX / 2);

            Width = boundsX;
            Height = boundsY;
        }
    }
}