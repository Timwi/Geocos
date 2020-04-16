using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class PointXCoordinateConstraint : Constraint
    {
        public Point Point { get; private set; }
        public double X { get; private set; }
        public PointXCoordinateConstraint(Point point, double x)
        {
            Point = point;
            X = x;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return Point.X - X;
        }
    }
}
