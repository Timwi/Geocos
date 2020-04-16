using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    abstract class GeometricObject
    {
        public abstract IEnumerable<Variable> GetVariables();
        public abstract IEnumerable<Polynomial> GetEquations();

        public abstract double GetX1(Dictionary<Variable, double> values);
        public abstract double GetY1(Dictionary<Variable, double> values);
        public abstract double GetX2(Dictionary<Variable, double> values);
        public abstract double GetY2(Dictionary<Variable, double> values);
        public abstract void Render(Graphics g, double x1, double y1, double x2, double y2, double factor, Func<double, float> toBitmapX, Func<double, float> toBitmapY, Dictionary<Variable, double> values);
    }
}
