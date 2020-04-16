using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class PointYCoordinateConstraint : Constraint
    {
        public Point Point { get; private set; }
        public double Y { get; private set; }
        public PointYCoordinateConstraint(Point point, double y)
        {
            Point = point;
            Y = y;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return Point.Y - Y;
        }
    }
}
