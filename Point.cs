using System.Drawing;
using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class Point : GeometricObject
    {
        public string Name { get; private set; }
        public Variable X { get; private set; }
        public Variable Y { get; private set; }

        public Point(string name)
        {
            Name = name;
            X = new Variable("x_" + name);
            Y = new Variable("y_" + name);
        }

        public override string ToString()
        {
            return "Point {0}".Fmt(Name);
        }

        public override IEnumerable<Variable> GetVariables() { return new[] { X, Y }; }
        public override IEnumerable<Polynomial> GetEquations() { return Enumerable.Empty<Polynomial>(); }

        public Constraint XCoordinate(double x) { return new PointXCoordinateConstraint(this, x); }
        public Constraint YCoordinate(double y) { return new PointYCoordinateConstraint(this, y); }

        public override double GetX1(Dictionary<Variable, double> values) { return values[X]; }
        public override double GetY1(Dictionary<Variable, double> values) { return values[Y]; }
        public override double GetX2(Dictionary<Variable, double> values) { return values[X]; }
        public override double GetY2(Dictionary<Variable, double> values) { return values[Y]; }

        public override void Render(Graphics g, double x1, double y1, double x2, double y2, double factor, Func<double, float> toBitmapX, Func<double, float> toBitmapY, Dictionary<Variable, double> values)
        {
            var x = toBitmapX(values[X]);
            var y = toBitmapY(values[Y]);
            g.FillEllipse(Brushes.Black, x - 3, y - 3, 6, 6);
            g.DrawString(Name, new Font("Georgia", 12, FontStyle.Bold), Brushes.Black, x + 5, y, new StringFormat { LineAlignment = StringAlignment.Center });
        }
    }
}
