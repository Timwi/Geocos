using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class CircleCircleTangentialConstraint : CircularTangentialConstraint
    {
        public Circle Circle1 { get; private set; }
        public Circle Circle2 { get; private set; }

        public CircleCircleTangentialConstraint(Circle circle1, Circle circle2, bool inside)
            : base(inside)
        {
            Circle1 = circle1;
            Circle2 = circle2;
        }

        public override Point Center1 { get { return Circle1.Center; } }
        public override Point Center2 { get { return Circle2.Center; } }
        public override Variable Radius1 { get { return Circle1.Radius; } }
        public override Variable Radius2 { get { return Circle2.Radius; } }
    }
}
