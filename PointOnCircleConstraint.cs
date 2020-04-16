using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class PointOnCircleConstraint : Constraint
    {
        public Point Point { get; private set; }
        public Circle Circle { get; private set; }

        public PointOnCircleConstraint(Point point, Circle circle)
        {
            Point = point;
            Circle = circle;
        }

        public override IEnumerable<Polynomial> GetEquations()
        {
            // Point must be Circle.Radius units away from Circle.Center
            yield return (Point.X - Circle.Center.X).Square() + (Point.Y - Circle.Center.Y).Square() - Circle.Radius.Square();
        }
    }
}
