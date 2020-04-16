using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util.ExtensionMethods;

namespace Geocos
{
    sealed class LineSegment : GeometricObject
    {
        public Point A { get; private set; }
        public Point B { get; private set; }

        public LineSegment(Point point1, Point point2)
        {
            A = point1;
            B = point2;
        }

        public override string ToString()
        {
            return "Line {0}{1}".Fmt(A, B);
        }

        public override IEnumerable<Variable> GetVariables()
        {
            yield break;
        }
        public override IEnumerable<Polynomial> GetEquations()
        {
            yield break;
        }

        public Constraint Horizontal() { return new LineSegmentHorizontalConstraint(this); }
        public Constraint Vertical() { return new LineSegmentVerticalConstraint(this); }
        public Constraint SpecificLength(double length) { return new LineSegmentLengthConstraint(this, length); }

        public override double GetX1(Dictionary<Variable, double> values) { return Math.Min(values[A.X], values[B.X]); }
        public override double GetY1(Dictionary<Variable, double> values) { return Math.Min(values[A.Y], values[B.Y]); }
        public override double GetX2(Dictionary<Variable, double> values) { return Math.Max(values[A.X], values[B.X]); }
        public override double GetY2(Dictionary<Variable, double> values) { return Math.Max(values[A.Y], values[B.Y]); }

        public override void Render(Graphics g, double x1, double y1, double x2, double y2, double factor, Func<double, float> toBitmapX, Func<double, float> toBitmapY, Dictionary<Variable, double> values)
        {
            g.DrawLine(new Pen(Brushes.Black, 2f), toBitmapX(values[A.X]), toBitmapY(values[A.Y]), toBitmapX(values[B.X]), toBitmapY(values[B.Y]));
        }
    }
}
