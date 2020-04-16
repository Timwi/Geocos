using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    abstract class CircularTangentialConstraint : Constraint
    {
        public abstract Point Center1 { get; }
        public abstract Point Center2 { get; }
        public abstract Variable Radius1 { get; }
        public abstract Variable Radius2 { get; }

        public bool Inside { get; private set; }

        public CircularTangentialConstraint(bool inside)
        {
            Inside = inside;
        }

        public sealed override IEnumerable<Polynomial> GetEquations()
        {
            yield return (Center1.X - Center2.X).Square() + (Center1.Y - Center2.Y).Square() - (
                Inside ? (Radius1 - Radius2).Square() : (Radius1 + Radius2).Square()
            );
        }
    }
}
