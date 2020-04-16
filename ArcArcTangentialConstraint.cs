using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class ArcArcTangentialConstraint : CircularTangentialConstraint
    {
        public Arc Arc1 { get; private set; }
        public Arc Arc2 { get; private set; }

        public ArcArcTangentialConstraint(Arc arc1, Arc arc2, bool inside)
            : base(inside)
        {
            Arc1 = arc1;
            Arc2 = arc2;
        }

        public override Point Center1 { get { return Arc1.Center; } }
        public override Point Center2 { get { return Arc2.Center; } }
        public override Variable Radius1 { get { return Arc1.Radius; } }
        public override Variable Radius2 { get { return Arc2.Radius; } }
    }
}
