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
        /*

        static void Main(string[] args)
        {
            //// -- Situation BEFORE rQ = (−yD^2 + 3.25 yD − xD^2 + 4.921875) / (−2xD + 5.5)
            //Variable yD = new Variable("a"), xD = new Variable("b"), yC = new Variable("c"), rQ = new Variable("d");
            //var origEquations = Ut.NewArray(
            //    yD.Square() - 3.25 * yD + xD.Square() - 3.25 * xD + 2.640625,
            //    -rQ.Square() * yC.Square() + 11.390625 * yC.Square() - 3.5 * rQ.Square() * yC + 39.8671875 * yC + 42.5 * rQ.Square() - 250.59375 * rQ + 379.4501953125,
            //    -2 * xD * rQ + 5.5 * rQ + yD.Square() - 3.25 * yD + xD.Square() - 4.921875,
            //    3 * yC.Square() * rQ.Square() - 16.5 * yC * rQ.Square() + 7.5 * rQ.Square() - 16.5 * yC.Square() * rQ + 90.75 * yC * rQ - 41.25 * rQ + yC.Pow(4) + 0.25 * yC.Pow(3) + 17.015625 * yC.Square() - 125.4921875 * yC + 64.8056640625
            //);

            // -- Situation AFTER rQ = (−yD^2 + 3.25 yD − xD^2 + 4.921875) / (−2xD + 5.5)
            Variable x_D = new Variable("x_D"), y_D = new Variable("y_D"), y_C = new Variable("y_C");
            var origEquations = Ut.NewArray(
                y_D.Square() - 3.25 * y_D + x_D.Square() - 3.25 * x_D + 2.640625,
                -y_C.Square() * y_D.Pow(4) - 3.5 * y_C * y_D.Pow(4) + 42.5 * y_D.Pow(4) + 6.5 * y_C.Square() * y_D.Pow(3) + 22.75 * y_C * y_D.Pow(3) - 276.25 * y_D.Pow(3) - 2 * y_C.Square() * x_D.Square() * y_D.Square() - 7 * y_C * x_D.Square() * y_D.Square() + 85 * x_D.Square() * y_D.Square() - 501.1875 * x_D * y_D.Square() - 0.71875 * y_C.Square() * y_D.Square() - 2.515625 * y_C * y_D.Square() + 1408.8125 * y_D.Square() + 6.5 * y_C.Square() * x_D.Square() * y_D + 22.75 * y_C * x_D.Square() * y_D - 276.25 * x_D.Square() * y_D + 1628.859375 * x_D * y_D - 31.9921875 * y_C.Square() * y_D - 111.97265625 * y_C * y_D - 3119.6953125 * y_D - y_C.Square() * x_D.Pow(4) - 3.5 * y_C * x_D.Pow(4) + 42.5 * x_D.Pow(4) - 501.1875 * x_D.Pow(3) + 55.40625 * y_C.Square() * x_D.Square() + 193.921875 * y_C * x_D.Square() + 2477.70703125 * x_D.Square() - 250.59375 * y_C.Square() * x_D - 877.078125 * y_C * x_D - 5881.1220703125 * x_D + 320.341552734375 * y_C.Square() + 1121.19543457031 * y_C + 5724.27355957031,
                3 * y_C.Square() * y_D.Pow(4) - 16.5 * y_C * y_D.Pow(4) + 7.5 * y_D.Pow(4) - 19.5 * y_C.Square() * y_D.Pow(3) + 107.25 * y_C * y_D.Pow(3) - 48.75 * y_D.Pow(3) + 6 * y_C.Square() * x_D.Square() * y_D.Square() - 33 * y_C * x_D.Square() * y_D.Square() + 15 * x_D.Square() * y_D.Square() - 33 * y_C.Square() * x_D * y_D.Square() + 181.5 * y_C * x_D * y_D.Square() - 82.5 * x_D * y_D.Square() + 92.90625 * y_C.Square() * y_D.Square() - 510.984375 * y_C * y_D.Square() + 232.265625 * y_D.Square() - 19.5 * y_C.Square() * x_D.Square() * y_D + 107.25 * y_C * x_D.Square() * y_D - 48.75 * x_D.Square() * y_D + 107.25 * y_C.Square() * x_D * y_D - 589.875 * y_C * x_D * y_D + 268.125 * x_D * y_D - 198.9609375 * y_C.Square() * y_D + 1094.28515625 * y_C * y_D - 497.40234375 * y_D + 3 * y_C.Square() * x_D.Pow(4) - 16.5 * y_C * x_D.Pow(4) + 7.5 * x_D.Pow(4) - 33 * y_C.Square() * x_D.Pow(3) + 181.5 * y_C * x_D.Pow(3) - 82.5 * x_D.Pow(3) + 4 * y_C.Pow(4) * x_D.Square() + y_C.Pow(3) * x_D.Square() + 129.28125 * y_C.Square() * x_D.Square() - 838.671875 * y_C * x_D.Square() + 412.26953125 * x_D.Square() - 22 * y_C.Pow(4) * x_D - 5.5 * y_C.Pow(3) * x_D - 211.921875 * y_C.Square() * x_D + 1867.5078125 * y_C * x_D - 1019.669921875 * x_D + 30.25 * y_C.Pow(4) + 7.5625 * y_C.Pow(3) + 140.737060546875 * y_C.Square() - 1739.21789550781 * y_C + 1025.40734863281
            );

            //// -- Example from the slides
            //var origEquations = Ut.NewArray(
            //    a.Square() + a * b + b.Square() - 7,
            //    a.Square() + a - b - 2,
            //    a.Square() + a * b - 3 * a - b - 8
            //);

            var equations = origEquations
                .Concat(origEquations.Select(eq => eq * x_D))
                .Concat(origEquations.Select(eq => eq * y_D))
                //.Concat(origEquations.Select(eq => eq * r_Q))
                .Concat(origEquations.Select(eq => eq * y_C))
                .ToArray();

            //equations = equations.Concat(
            //    equations.Select(eq => eq * x_D)).Concat(
            //    equations.Select(eq => eq * y_D)).Concat(
            //    //equations.Select(eq => eq * r_Q)).Concat(
            //    equations.Select(eq => eq * y_C)).ToArray();

            var columns = equations.SelectMany(eq => eq.Coefficients.Select((_, i) => new
            {
                VarString = eq.CoefficientVariablesAsString(i),
                VarPoly = eq.CoefficientVariablesAsPolynomial(i)
            }).Where((_, i) => eq.Coefficients[i] != 0)).DistinctBy(inf => inf.VarString).OrderByDescending(x => x.VarPoly.Degree).ThenByDescending(x => x.VarPoly.Variables.Length).ToArray();

            var cells = Ut.NewArray(equations.Length, rowIndex => Ut.NewArray(columns.Length, columnIndex =>
                equations[rowIndex].Coefficients.Where((_, coeffIndex) => equations[rowIndex].CoefficientVariablesAsString(coeffIndex) == columns[columnIndex].VarString).FirstOrDefault()
            ));

            var columnStrings = columns.Select(cl => cl.VarString).ToArray();
            var rowStrings = Enumerable.Range(0, equations.Length).Select(i => i.ToString()).ToArray();


            // BEGIN ALGORITHM
            int col = 0, row = 0, cols = columns.Length, rows = equations.Length, iteration = 0;
            while (true)
            {
                Console.WriteLine("Writing iteration {0}...".Fmt(iteration));
                GeoUt.OutputTable(columnStrings, rowStrings, cells.Select(rw => rw.Select(dbl => dbl == 0 ? null : ExactConvert.ToString(dbl)).ToArray()).ToArray(),
                    @"YL-{0}.htm".Fmt(iteration + 1),
                    @"D:\Daten\Upload\Geocos\YL-{0}.htm".Fmt(iteration));

                if (col >= cols || row >= rows)
                    break;

                var equationToUse = Enumerable.Range(row, rows - row).FirstOrDefault(rw => cells[rw][col] != 0, -1);
                if (equationToUse == -1)
                    goto IncCol;

                if (equationToUse != row)
                    Ut.Swap(ref cells[equationToUse], ref cells[row]);

                var divideBy = cells[row][col];
                if (divideBy != 1)
                    for (int cl = col; cl < cols; cl++)
                        cells[row][cl] /= divideBy;

                for (int r = row + 1; r < rows; r++)
                {
                    if (cells[r][col] == 0)
                        continue;
                    var multiplier = cells[r][col];
                    for (int cl = col; cl < cols; cl++)
                        cells[r][cl] -= cells[row][cl] * multiplier;
                }

                iteration++;
                row++;
                IncCol: col++;
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        /*/

        static void Main()
        {
            try { Console.OutputEncoding = Encoding.UTF8; }
            catch { }

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
            Func<Variable, double> evalFunc = null;
            evalFunc = v => solutionsRaw[v].Evaluate(evalFunc);
            var solutions = solutionsRaw.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Evaluate(evalFunc));

            Render(objects, solutions).Save(@"D:\Daten\Upload\Geocos\Result.png");

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        /**/
    }
}
