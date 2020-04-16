using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class Fraction : Formula
    {
        public Formula Numerator { get; private set; }
        public Formula Denominator { get; private set; }

        private Fraction() { }

        public static Formula Create(Formula numerator, Formula denominator)
        {
            if (denominator == null)
                return numerator;
            if (numerator is Polynomial && denominator is Polynomial && ((Polynomial) denominator).Degree == 0)
                return ((Polynomial) numerator) / ((Polynomial) denominator).Coefficients[0];
            return new Fraction
            {
                Numerator = numerator,
                Denominator = denominator
            };
        }

        public override Formula Substitute(Variable variable, Formula value)
        {
            return Create(
                numerator: Numerator.Substitute(variable, value),
                denominator: Denominator.Substitute(variable, value));
        }

        public override double Evaluate(Func<Variable, double> func)
        {
            var numerEval = Numerator.Evaluate(func);
            if (numerEval == 0)
                return 0;
            return numerEval / Denominator.Evaluate(func);
        }

        public override string ToMaple()
        {
            return "({0})/({1})".Fmt(Numerator.ToMaple(), Denominator.ToMaple());
        }

        public override string ToString()
        {
            return @"\frac{{ {0} }}{{ {1} }}".Fmt(Numerator, Denominator);
        }
    }
}
