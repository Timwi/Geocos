using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class LineSegmentVerticalConstraint : Constraint
    {
        public LineSegment LineSegment { get; private set; }
        public LineSegmentVerticalConstraint(LineSegment lineSegment) { LineSegment = lineSegment; }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return LineSegment.A.X - LineSegment.B.X;
        }
    }
}
