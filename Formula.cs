using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    abstract class Formula
    {
        public abstract string ToMaple();
        public abstract override string ToString();
        public abstract Formula Substitute(Variable variable, Formula value);
        public abstract double Evaluate(Func<Variable, double> func);

        public static implicit operator Formula(Variable variable)
        {
            return new Polynomial(1, new[] { variable }, 0, 1);
        }

        public static implicit operator Formula(double value)
        {
            return new Polynomial(0, new Variable[] { }, value);
        }
    }
}
