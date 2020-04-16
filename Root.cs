using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class Root : Formula
    {
        public Formula Inner { get; private set; }
        public bool Negative { get; private set; }

        public Root(Formula inner, bool negative)
        {
            Inner = inner;
            Negative = negative;
        }

        public override Formula Substitute(Variable variable, Formula value)
        {
            return new Root(Inner.Substitute(variable, value), Negative);
        }

        public override double Evaluate(Func<Variable, double> func)
        {
            var sqrt = Math.Sqrt(Inner.Evaluate(func));
            return Negative ? -sqrt : sqrt;
        }

        public override string ToMaple()
        {
            return @"{0}sqrt({1})".Fmt(Negative ? "-" : "", Inner.ToMaple());
        }

        public override string ToString()
        {
            return @"{0}\sqrt{{ {1} }}".Fmt(Negative ? "-" : "", Inner);
        }

        // CODE TO PLUG THIS VALUE INTO OTHER POLYNOMIALS
        // 
        //// Any equation that contains 𝑥, e.g.
        ////   𝑗 𝑥⁴ + 𝑘 𝑥³ + 𝑚 𝑥² + 𝑛 𝑥 + 𝑝 = 0
        //// must be rearranged as follows:
        ////   [ 𝑗 (𝑥²)² + 𝑚 (𝑥²) + 𝑝 ] + 𝑥 (𝑘 (𝑥²) + 𝑛) = 0
        ////   [ 𝑗 (𝑥²)² + 𝑚 (𝑥²) + 𝑝 ] = −𝑥 (𝑘 (𝑥²) + 𝑛)
        ////   [ 𝑗 (𝑥²)² + 𝑚 (𝑥²) + 𝑝 ]² = 𝑥² (𝑘 (𝑥²) + 𝑛)²
        ////   [ 𝑗 (𝑥²)² + 𝑚 (𝑥²) + 𝑝 ]² − 𝑥² (𝑘 (𝑥²) + 𝑛)² = 0
        //// and then all the (𝑥²)s are replaced with 𝑐/(−𝑎).
        //var newEquations = new Polynomial[equations.Length - 1];
        //for (int i = 0; i < equations.Length; i++)
        //{
        //    if (i == eqIndex)
        //        continue;

        //    var xIndex = equations[i].Variables.IndexOf(solveFor);
        //    var j = i > eqIndex ? i - 1 : i;
        //    if (xIndex == -1)
        //    {
        //        newEquations[j] = equations[i];
        //        continue;
        //    }

        //    var terms = equations[i].CollectTerms(xIndex);
        //    //   [ 𝑗 (𝑥²)² + 𝑚 (𝑥²) + 𝑝 ]² + 𝑥² (𝑘 (𝑥²) + 𝑛)² = 0
        //    //        𝑙² + 𝑥²𝑟² = 0
        //    Polynomial l = 0, r = 0;
        //    for (int p = 0; p < terms.Length; p++)
        //    {
        //        if ((p & 1) == 0)
        //            l += terms[p] * xsq.Pow(p >> 1);
        //        else
        //            r += terms[p] * xsq.Pow(p >> 1);
        //    }
        //    newEquations[j] = l.Square() - xsq * r.Square();
        //}
    }
}
