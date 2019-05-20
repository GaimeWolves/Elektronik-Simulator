using OESim.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OESim.Circuit.Visualization
{
    public struct Wire
    {
        private Point p1, p2;

        public Path Path { get; }
        public Point P1
        {
            get => p1;
            set
            {
                p1 = value;
                PathUtils.UpdatePath(Path, p1, p2);
            }
        }
        public Point P2
        {
            get => p2;
            set
            {
                p2 = value;
                PathUtils.UpdatePath(Path, p1, p2);
            }
        }

        public Wire(Point p1, Point p2)
        {
            Path = PathUtils.CreatePath(p1, p2);
            this.p1 = p1;
            this.p2 = p2;
        }

        public Wire(Point p1, Point p2, TransformGroup transform)
        {
            Path = PathUtils.CreatePath(p1, p2);
            Path.RenderTransform = transform;
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}
