using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class StraightLinesParallelConstraint : Constraint
    {
        public StraightLine Line1 { get; private set; }
        public StraightLine Line2 { get; private set; }
        public double? Distance { get; private set; }

        public StraightLinesParallelConstraint(StraightLine line1, StraightLine line2, double? distance)
        {
            Line1 = line1;
            Line2 = line2;
            Distance = distance;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return Line1.DirectionVector.X - Line2.DirectionVector.X;
            if (Distance != null)
                yield return Line2.DistanceFromOrigin - Line1.DistanceFromOrigin - Distance.Value;
        }
    }
}
