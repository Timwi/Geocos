using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class Arc : GeometricObject
    {
        public string Name { get; private set; }
        public Point Center { get; private set; }
        public Point A { get; private set; }
        public Point B { get; private set; }
        public Variable Radius { get; private set; }

        public Arc(string name, Point center, Point a, Point b)
        {
            Name = name;
            Center = center;
            A = a;
            B = b;
            Radius = new Variable("r_" + name, nonNegative: true);
        }

        public override IEnumerable<Variable> GetVariables() { yield return Radius; }
        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return (Center.X - A.X).Square() + (Center.Y - A.Y).Square() - Radius.Square();
            yield return (Center.X - B.X).Square() + (Center.Y - B.Y).Square() - Radius.Square();
        }

        public override double GetX1(Dictionary<Variable, double> values) { return values[Center.X] - values[Radius]; }
        public override double GetY1(Dictionary<Variable, double> values) { return values[Center.Y] - values[Radius]; }
        public override double GetX2(Dictionary<Variable, double> values) { return values[Center.X] + values[Radius]; }
        public override double GetY2(Dictionary<Variable, double> values) { return values[Center.Y] + values[Radius]; }

        public override void Render(Graphics g, double x1, double y1, double x2, double y2, double factor, Func<double, float> toBitmapX, Func<double, float> toBitmapY, Dictionary<Variable, double> values)
        {
            var ax = values[A.X];
            var ay = values[A.Y];
            var bx = values[B.X];
            var by = values[B.Y];
            var cx = values[Center.X];
            var cy = values[Center.Y];
            var r = values[Radius];

            var startAngle = (float) (Math.Atan2(ay - cy, ax - cx) * 180 / Math.PI);
            var endAngle = (float) (Math.Atan2(by - cy, bx - cx) * 180 / Math.PI);
            var br = (float) (2 * r * factor);

            g.DrawArc(new Pen(Brushes.Black, 2f), toBitmapX(cx - r), toBitmapY(cy - r), br, br, startAngle, (endAngle - startAngle + 360) % 360);
        }
    }
}
