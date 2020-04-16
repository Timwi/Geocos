using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class PointOnArcConstraint : Constraint
    {
        public Point Point { get; private set; }
        public Arc Arc { get; private set; }
        //public double? Ratio { get; private set; }

        public PointOnArcConstraint(Point point, Arc arc, double? ratio = null)
        {
            Point = point;
            Arc = arc;
            //Ratio = ratio;
            if (ratio != null)
                throw new NotImplementedException("Ratio in PointOnArcConstraint not yet implemented.");
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            // Point must be Arc.Radius units away from Arc.Center
            yield return (Point.X - Arc.Center.X).Square() + (Point.Y - Arc.Center.Y).Square() - Arc.Radius.Square();
        }
    }
}
