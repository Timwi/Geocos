using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class StraightLineDistanceConstraint : Constraint
    {
        public StraightLine StraightLine { get; private set; }
        public double DistanceFromOrigin { get; private set; }
        public StraightLineDistanceConstraint(StraightLine straightLine, double distance)
        {
            StraightLine = straightLine;
            DistanceFromOrigin = distance;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return StraightLine.DistanceFromOrigin - DistanceFromOrigin;
        }
    }
}
