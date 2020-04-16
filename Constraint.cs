using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    abstract class Constraint
    {
        public abstract IEnumerable<Polynomial> GetEquations();
    }
}
