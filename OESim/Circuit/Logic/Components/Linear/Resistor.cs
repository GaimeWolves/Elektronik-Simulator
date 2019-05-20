using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace OESim.Circuit.Logic.Components.Linear
{
    public class Resistor : IEComponent
    {
        private static readonly string PathGeometry = "M 0,-2 V -1.25 H -0.5 V 1.25 H 0.5 V -1.25 H 0 M 0,1.25 V 2";

        private Dictionary<string, object> _Attributes;

        public Resistor()
        {
            _Attributes = new Dictionary<string, object>();
            _Attributes.Add("BoundsX", 1.0);
            _Attributes.Add("BoundsY", 4.0);
            _Attributes.Add("Name", "Resistor");
        }

        public Path CreateVisual()
        {
            Path path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 0.05;
            path.Data = Geometry.Parse(PathGeometry);
            path.Stretch = Stretch.Uniform;
            return path;
        }

        public T GetAttribute<T>(string name)
        {
            try
            {
                return (T)_Attributes[name];
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show("Attribute not of Type " + typeof(T).FullName + "!\n" + ex.ToString());
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("No such Attribute!\n" + ex.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unknown Exception!\n" + ex.ToString());
            }
            return default;
        }

        public void SetAttribute<T>(string name, T value)
        {
            try
            {
                _Attributes[name] = value;
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("No such Attribute!\n" + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown Exception!\n" + ex.ToString());
            }
        }
    }
}
