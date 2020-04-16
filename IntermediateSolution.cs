using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class IntermediateSolution
    {
        public Variable Variable { get; private set; }
        public Polynomial Numerator { get; private set; }
        public Polynomial Denominator { get; private set; }

        public IntermediateSolution(Variable variable, Polynomial numerator, Polynomial denominator)
        {
            Variable = variable;
            Numerator = numerator;
            Denominator = denominator;
        }
    }
}
