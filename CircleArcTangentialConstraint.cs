using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class CircleArcTangentialConstraint : CircularTangentialConstraint
    {
        public Circle Circle { get; private set; }
        public Arc Arc { get; private set; }

        public CircleArcTangentialConstraint(Circle circle, Arc arc, bool inside)
            : base(inside)
        {
            Circle = circle;
            Arc = arc;
        }

        public override Point Center1 { get { return Circle.Center; } }
        public override Point Center2 { get { return Arc.Center; } }
        public override Variable Radius1 { get { return Circle.Radius; } }
        public override Variable Radius2 { get { return Arc.Radius; } }
    }
}
