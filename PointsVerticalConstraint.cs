using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class PointsVerticalConstraint : Constraint
    {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public PointsVerticalConstraint(Point a, Point b)
        {
            A = a;
            B = b;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return A.X - B.X;
        }
    }
}
