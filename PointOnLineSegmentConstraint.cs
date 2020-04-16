using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class PointOnLineSegmentConstraint : Constraint
    {
        public Point Point { get; private set; }
        public LineSegment LineSegment { get; private set; }
        public double? Ratio { get; private set; }

        public PointOnLineSegmentConstraint(Point point, LineSegment lineSegment, double? ratio = null)
        {
            Point = point;
            LineSegment = lineSegment;
            Ratio = ratio;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            if (Ratio == null)
            {
                var x1 = LineSegment.A.X;
                var y1 = LineSegment.A.Y;
                var x2 = LineSegment.B.X;
                var y2 = LineSegment.B.Y;
                // yield return Point.Y * (x2 - x1) - Point.X * (y2 - y1) + (x2 * y1 - x1 * y2);

                // (𝑦 − 𝑦₁) (𝑥₂ − 𝑥₁) + (𝑥₁ − 𝑥) (𝑦₂ − 𝑦₁) = 0
                yield return (Point.Y - y1) * (x2 - x1) + (x1 - Point.X) * (y2 - y1);
            }
            else
            {
                yield return Point.X - LineSegment.A.X - Ratio.Value * (LineSegment.B.X - LineSegment.A.X);
                yield return Point.Y - LineSegment.A.Y - Ratio.Value * (LineSegment.B.Y - LineSegment.A.Y);
            }
        }
    }
}
