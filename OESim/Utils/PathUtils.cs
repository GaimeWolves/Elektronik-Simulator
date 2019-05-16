using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OESim.Utils
{
    public class PathUtils
    {
        private static readonly CultureInfo English = new CultureInfo("en-US");

        public static Path CreatePath(List<Point> points)
        {
            string data = CreateDataString(points);

            return new Path
            {
                StrokeThickness = 0.05,
                Stroke = Brushes.Black,
                Data = Geometry.Parse(data)
            };
        }

        public static Path CreatePath(params Point[] points) => CreatePath(points.ToList());

        public static void UpdatePath(Path path, List<Point> points)
        {
            string data = CreateDataString(points);
            path.Data = Geometry.Parse(data);
        }

        public static void UpdatePath(Path path, params Point[] points) => UpdatePath(path, points.ToList());

        public static Point TransformPoint(Point point, Point offset, double scale)
        {
            return new Point
            {
                X = (point.X + offset.X) * scale,
                Y = (point.Y + offset.Y) * scale
            };
        }

        private static string CreateDataString(List<Point> points)
        {
            if (points.Count < 1) throw new ArgumentException("A path requires at least one point.");

            string data = "M " + points[0].ToString(English);
            points.RemoveAt(0);

            points.ForEach(p => data += " L " + p.ToString(English));

            return data;
        }
    }
}
