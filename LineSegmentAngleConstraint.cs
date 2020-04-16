using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class LineSegmentAngleConstraint : Constraint
    {
        public LineSegment LineSegment { get; private set; }
        public double Angle { get; private set; }
        public LineSegmentAngleConstraint(LineSegment lineSegment, double angle)
        {
            LineSegment = lineSegment;
            Angle = angle;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            if (Angle == 90 || Angle == 270)
                yield return LineSegment.A.X - LineSegment.B.X;
            else
            {
                var a = LineSegment.A;
                var b = LineSegment.B;
                yield return (a.Y - b.Y) - (a.X - b.X) * Math.Tan(Angle / 180.0 * Math.PI);
            }
        }
    }
}
