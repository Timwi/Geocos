using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class StraightLineHorizontalConstraint : Constraint
    {
        public StraightLine StraightLine { get; private set; }
        public StraightLineHorizontalConstraint(StraightLine straightLine) { StraightLine = straightLine; }

        public override IEnumerable<Polynomial> GetEquations()
        {
            yield return StraightLine.DirectionVector.Y;    // = 0
        }
    }
}
