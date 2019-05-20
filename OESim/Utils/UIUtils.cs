using OESim.Circuit.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OESim.Utils
{
    public static class UIUtils
    {
        public static Button CreateComponentButton(Component component, string name, int size)
        {
            Button ret = new Button();
            ret.Name = name;
            ret.MinHeight = size;
            ret.MaxHeight = size;
            ret.MinWidth = size;
            ret.MaxWidth = size;
            ret.Background = null;

            Grid grid = new Grid();
            grid.MinHeight = size;
            grid.MaxHeight = size;
            grid.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Viewbox cView = new Viewbox();
            Grid.SetRow(cView, 0);
            Grid.SetColumn(cView, 0);
            cView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            cView.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            cView.Stretch = Stretch.Uniform;
            component.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            component.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            component.RenderTransform = new TranslateTransform(component.Width / 2, component.Height / 2);
            cView.Child = component;
            grid.Children.Add(cView);

            Viewbox view = new Viewbox();
            Grid.SetRow(view, 1);
            Grid.SetColumn(view, 0);

            view.Child = new TextBlock()
            {
                Text = name,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };

            grid.Children.Add(view);
            ret.Content = grid;

            return ret;
        }
    }
}
