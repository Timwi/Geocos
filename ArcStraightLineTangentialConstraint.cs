using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class ArcStraightLineTangentialConstraint : Constraint
    {
        public Arc Arc { get; private set; }
        public StraightLine StraightLine { get; private set; }

        public ArcStraightLineTangentialConstraint(Arc arc, StraightLine straightLine)
        {
            Arc = arc;
            StraightLine = straightLine;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return StraightLine.DirectionVector.X * Arc.Center.Y - StraightLine.DirectionVector.Y * Arc.Center.X - (StraightLine.DistanceFromOrigin + Arc.Radius);
        }
    }
}
