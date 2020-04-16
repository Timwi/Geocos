using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class LineSegmentLengthConstraint : Constraint
    {
        public LineSegment LineSegment { get; private set; }
        public double Length { get; private set; }
        public LineSegmentLengthConstraint(LineSegment lineSegment, double length)
        {
            LineSegment = lineSegment;
            Length = length;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            var a = LineSegment.A;
            var b = LineSegment.B;
            yield return (a.Y - b.Y).Square() + (a.X - b.X).Square() - (Length * Length);
        }
    }
}
