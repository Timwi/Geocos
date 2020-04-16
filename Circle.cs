using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class Circle : GeometricObject
    {
        public string Name { get; private set; }
        public Point Center { get; private set; }
        public Variable Radius { get; private set; }

        public Circle(string name, Point center)
        {
            Name = name;
            Center = center;
            Radius = new Variable("r_" + name, nonNegative: true);
        }

        public override IEnumerable<Variable> GetVariables() { yield return Radius; }
        public override IEnumerable<Polynomial> GetEquations() { return Enumerable.Empty<Polynomial>(); }

        public override double GetX1(Dictionary<Variable, double> values) { return values[Center.X] - values[Radius]; }
        public override double GetY1(Dictionary<Variable, double> values) { return values[Center.Y] - values[Radius]; }
        public override double GetX2(Dictionary<Variable, double> values) { return values[Center.X] + values[Radius]; }
        public override double GetY2(Dictionary<Variable, double> values) { return values[Center.Y] + values[Radius]; }

        public override void Render(Graphics g, double x1, double y1, double x2, double y2, double factor, Func<double, float> toBitmapX, Func<double, float> toBitmapY, Dictionary<Variable, double> values)
        {
            var r = values[Radius];
            var x = values[Center.X];
            var y = values[Center.Y];
            var br = (float) (2 * r * factor);
            g.DrawEllipse(new Pen(Brushes.Black, 2f), toBitmapX(x - r), toBitmapY(y - r), br, br);
        }
    }
}
