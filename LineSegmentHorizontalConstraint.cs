using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class LineSegmentHorizontalConstraint : Constraint
    {
        public LineSegment LineSegment { get; private set; }
        public LineSegmentHorizontalConstraint(LineSegment lineSegment) { LineSegment = lineSegment; }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return LineSegment.A.Y - LineSegment.B.Y;
        }
    }
}
