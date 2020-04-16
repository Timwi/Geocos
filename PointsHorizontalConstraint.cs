using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class PointsHorizontalConstraint : Constraint
    {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public PointsHorizontalConstraint(Point a, Point b)
        {
            A = a;
            B = b;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return A.Y - B.Y;
        }
    }
}
