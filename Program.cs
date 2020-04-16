using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RT.TagSoup;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace Geocos
{
    partial class Program
    {
        static void Main()
        {
            try { Console.OutputEncoding = Encoding.UTF8; }
            catch { }

            VeryComplicated6();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static void VeryComplicated6()
        {
            var A = new Point("A");
            var B = new Point("B");
            var C = new Point("C");
            var D = new Point("D");
            var E = new Point("E");
            var F = new Point("F");
            var G = new Point("G");
            var H = new Point("H");
            var J = new Point("J");
            var K = new Point("K");
            var L = new Point("L");

            var M = new Circle("M", H);
            var N = new Arc("N", center: H, a: L, b: D);
            var P = new Arc("P", center: K, a: C, b: D);
            var Q = new Arc("Q", center: K, a: A, b: L);
            var R = new StraightLine("R");
            var S = new StraightLine("S");

            var AC = new LineSegment(A, C);
            var EG = new LineSegment(E, G);

            var objects = Ut.NewArray<GeometricObject>(A, B, C, D, E, F, G, H, J, K, L, M, N, P, Q, R, S, AC, EG);

            var constraints = Ut.NewArray<Constraint>(
                new PointOnStraightLineConstraint(A, R),
                new PointsVerticalConstraint(B, J),
                new PointOnLineSegmentConstraint(B, AC, ratio: 0.5),
                new PointOnArcConstraint(E, N),
                new PointOnLineSegmentConstraint(F, EG, ratio: 0.5),
                new PointOnStraightLineConstraint(F, S, distance: 3),
                new PointOnCircleConstraint(G, M),
                new PointsHorizontalConstraint(H, L),
                new PointsHorizontalConstraint(H, J),
                new PointOnLineSegmentConstraint(H, EG),
                new PointOnCircleConstraint(J, M),
                new PointOnLineSegmentConstraint(K, AC),
                new StraightLinesParallelConstraint(R, S, distance: -5),
                new StraightLineHorizontalConstraint(R),
                new CircleArcTangentialConstraint(M, P, inside: true),
                new ArcStraightLineTangentialConstraint(N, S),
                new ArcArcTangentialConstraint(N, Q, inside: true),
                new LineSegmentLengthConstraint(EG, 0.5),
                new PointXCoordinateConstraint(L, 0),
                new StraightLineHorizontalConstraint(S),
                new StraightLineDistanceConstraint(S, 0),
                new LineSegmentVerticalConstraint(EG),
                new PointsHorizontalConstraint(K, H)
            );

            var variables = objects.SelectMany(o => o.GetVariables());
            var equations = Solver.GetEquations(constraints, objects).ToList();
            //equations.Add((D.X - 2.75) * Q.Radius - D.X * 1.625 + 3.78125);
            var solutionsRaw = Solver.Solve(equations.ToArray()).ToDictionary(tup => tup.Item1, tup => tup.Item2);
            double evalFunc(Variable v) => solutionsRaw[v].Evaluate(evalFunc);
            var solutions = solutionsRaw.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Evaluate(evalFunc));

            Render(objects, solutions).Save(@"D:\Daten\Upload\Geocos\Result.png");
        }

        /**/
    }
}
