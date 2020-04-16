using System.Drawing;
using System.Drawing.Drawing2D;
using System;
using System.Collections.Generic;
using RT.Util.ExtensionMethods;

namespace Geocos
{
    sealed class StraightLine : GeometricObject
    {
        public string Name { get; private set; }
        public Point DirectionVector { get; private set; }
        public Variable DistanceFromOrigin { get; private set; }

        public StraightLine(string name)
        {
            Name = name;
            DirectionVector = new Point(name);
            DistanceFromOrigin = new Variable("d_" + name, nonNegative: true);
        }

        public override string ToString()
        {
            return "Straight Line {0}".Fmt(Name);
        }

        public override IEnumerable<Variable> GetVariables() { yield return DistanceFromOrigin; }
        public override IEnumerable<Polynomial> GetEquations() { yield return DirectionVector.X.Square() + DirectionVector.Y.Square() - 1; }

        public Constraint Horizontal() { return new StraightLineHorizontalConstraint(this); }
        public Constraint Vertical() { return new StraightLineVerticalConstraint(this); }

        public override double GetX1(Dictionary<Variable, double> values) { return 0; }
        public override double GetY1(Dictionary<Variable, double> values) { return 0; }
        public override double GetX2(Dictionary<Variable, double> values) { return 0; }
        public override double GetY2(Dictionary<Variable, double> values) { return 0; }

        public override void Render(Graphics g, double x1, double y1, double x2, double y2, double factor, Func<double, float> toBitmapX, Func<double, float> toBitmapY, Dictionary<Variable, double> values)
        {
            if (values[DirectionVector.Y] == 0)
            {
                // vertical line
                var bx = toBitmapX(values[DistanceFromOrigin]);
                g.DrawLine(new Pen(Brushes.LightGray) { DashStyle = DashStyle.Dash }, bx, toBitmapY(y1) - 5, bx, toBitmapY(y2) + 5);
                g.DrawString(Name, new Font("Georgia", 12, FontStyle.Bold), Brushes.LightGray, bx + 5, toBitmapY(y1));
            }
            else
            {
                var d = values[DistanceFromOrigin];
                var cosθ = values[DirectionVector.X];
                var sinθ = values[DirectionVector.Y];
                var ys = (d - x1 * cosθ) / sinθ;
                var ye = (d - x2 * cosθ) / sinθ;

                g.DrawLine(new Pen(Brushes.LightGray) { DashStyle = DashStyle.Dash }, toBitmapX(x1), toBitmapY(ys), toBitmapX(x2), toBitmapY(ye));
                if (ys < y1)
                    g.DrawString(Name, new Font("Georgia", 12, FontStyle.Bold), Brushes.LightGray, toBitmapX((d - y1 * sinθ) / cosθ), toBitmapY(y1));
                else
                    g.DrawString(Name, new Font("Georgia", 12, FontStyle.Bold), Brushes.LightGray, toBitmapX(x1) + 5, toBitmapY(ys));
            }
        }
    }
}
