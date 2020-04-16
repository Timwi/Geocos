using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;

namespace Geocos
{
    sealed class PointOnStraightLineConstraint : Constraint
    {
        public Point Point { get; private set; }
        public StraightLine StraightLine { get; private set; }
        public double Distance { get; private set; }

        public PointOnStraightLineConstraint(Point point, StraightLine straightLine, double distance = 0)
        {
            Point = point;
            StraightLine = straightLine;
            Distance = distance;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return StraightLine.DirectionVector.X * Point.Y - StraightLine.DirectionVector.Y * Point.X - (StraightLine.DistanceFromOrigin + Distance);
        }
    }
}
