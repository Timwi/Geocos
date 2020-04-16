using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.Collections;
using RT.Util.ExtensionMethods;

namespace Geocos
{
    static class Solver
    {
        public static IEnumerable<Polynomial> GetEquations(IEnumerable<Constraint> constraints, params GeometricObject[] objects)
        {
            return constraints.SelectMany(c => c.GetEquations())
                .Concat(objects.SelectMany(o => o.GetEquations()));
        }

        public static IEnumerable<Tuple<Variable, Formula>> Solve(Polynomial[] equations)
        {
            var knowns = new List<Tuple<Variable, Formula>>();
            var id = "Start";

            while (true)
            {
                GeoUt.GenerateHtml(equations, knowns, id, null, 0, null);

                if (equations.Length == 0)
                    return knowns;

                var step = SolveOneStep(equations).First();

                var done = step.IfType(
                    (SolvedForVariable ss) =>
                    {
                        var nextId = "{0}›{1}".Fmt(id, ss.SolvedFor.Name);
                        GeoUt.GenerateHtml(equations, knowns, id, nextId, ss.EquationIndex.Value, "$ {0} = {1} $".Fmt(ss.SolvedFor, ss.Solution));
                        id = nextId;
                        equations = equations.Where((eq, i) => i != ss.EquationIndex).Select(eq => eq.SubstituteAsEquation(ss.SolvedFor, ss.Solution)).ToArray();
                        knowns = knowns.Select(kn => Tuple.Create(kn.Item1, kn.Item2.Substitute(ss.SolvedFor, ss.Solution))).ToList();
                        knowns.Add(Tuple.Create(ss.SolvedFor, ss.Solution));
                        return false;
                    },
                    (RedundantEquation ss) =>
                    {
                        var nextId = "{0}ǀ{1}".Fmt(id, ss.EquationIndex);
                        GeoUt.GenerateHtml(equations, knowns, id, nextId, ss.EquationIndex.Value, "Redundant equation");
                        id = nextId;
                        equations = equations.Where((eq, i) => i != ss.EquationIndex).ToArray();
                        return false;
                    },
                    (NewEquations ss) =>
                    {
                        var nextId = "{0}˟".Fmt(id);
                        GeoUt.GenerateHtml(equations, knowns, id, nextId, 0, "XL Algorithm");
                        id = nextId;
                        equations = ss.Equations;
                        return false;
                    },
                    ss =>
                    {
                        throw new NotImplementedException();
                    }
                );

                if (done)
                    return knowns;
            }
        }

        public static IEnumerable<SolveStep> SolveOneStep(Polynomial[] equations)
        {
            if (equations.Length == 0)
                yield break;

            var redundantEquationIndex = equations.IndexOf(eq => eq == 0);
            if (redundantEquationIndex != -1)
            {
                yield return new RedundantEquation(redundantEquationIndex);
                yield break;
            }

            var contradictionIndex = equations.IndexOf(eq => eq.Degree == 0);
            if (contradictionIndex != -1)
            {
                yield return new NoSolution(contradictionIndex);
                yield break;
            }

            var extremelySimpleEquationIndex = equations.IndexOf(eq => eq.Degree == 1 && eq.Variables.Length < 2);
            if (extremelySimpleEquationIndex != -1)
            {
                var extremelySimpleEquation = equations[extremelySimpleEquationIndex];
                var terms = extremelySimpleEquation.CollectTerms(0);
                yield return new SolvedForVariable(extremelySimpleEquationIndex, extremelySimpleEquation.Variables[0], Fraction.Create(-terms[0], terms[1]));
                yield break;
            }

            //var simpleQuadraticIndex = equations.IndexOf(eq => eq.Degree == 2 && eq.Variables.Length < 2);
            //if (simpleQuadraticIndex != -1)
            //{
            //    var simpleQuadratic = equations[simpleQuadraticIndex];
            //    var x = simpleQuadratic.Variables[0];
            //    var a = simpleQuadratic.Coefficients[2];
            //    var b = simpleQuadratic.Coefficients[1];
            //    var c = simpleQuadratic.Coefficients[0];
            //    var det = b * b - 4 * a * c;
            //    if (det < 0)
            //    {
            //        // No solution if 𝑏² − 4 𝑎 𝑐 is negative
            //        yield return new NoSolution(simpleQuadraticIndex);
            //        yield break;
            //    }

            //    var sols = new[] { (-b + Math.Sqrt(det)) / (2 * a), (-b - Math.Sqrt(det)) / (2 * a) }.Distinct();
            //    if (x.NonNegative)
            //        sols = sols.Where(sol => sol >= 0);
            //    var solArr = sols.ToArray();
            //    if (solArr.Length == 0)
            //    {
            //        yield return new NoSolution(simpleQuadraticIndex);
            //        yield break;
            //    }

            //    Array.Sort(solArr, (one, two) => Math.Abs(one - x.SuggestedValue).CompareTo(Math.Abs(two - x.SuggestedValue)));

            //    //if (x.Name == "x_J")
            //    //    Ut.Swap(ref solArr[0], ref solArr[1]);

            //    foreach (var sol in solArr)
            //        yield return new SolvedForVariable(simpleQuadraticIndex, x, sol);
            //    yield break;
            //}

            // Find an equation we can simplify by dividing by one of its variables
            for (int eqIndex = 0; eqIndex < equations.Length; eqIndex++)
            {
                var eq = equations[eqIndex];

                // If the constant term is 0, we may be able to do this
                Tuple<Polynomial, Variable> stuff;
                if ((eq.Coefficients[0] == 0) && (stuff = eq.Decimate()) != null)
                {
                    var simplerEquation = stuff.Item1;
                    var variable = stuff.Item2;

                    // Solution 1: variable = 0
                    yield return new SolvedForVariable(eqIndex, variable, 0);

                    // Solution 2: the same set of equations, except with this one simplified
                    // (only exists if there are any other variables left in the equation;
                    // otherwise the remaining equation is just “1 = 0” which obviously has no solutions)
                    if (simplerEquation.Variables.Length > 0)
                        yield return new EquationSimplified(eqIndex, simplerEquation);

                    yield break;
                }
            }

            //// Find a single-variable equation and try Newton approximation on it
            //var singleVariableIndex = equations.IndexOf(eq => eq.Variables.Length == 1);
            //if (singleVariableIndex != -1)
            //{
            //    var maxIterations = 1000;
            //    var threshold = 0.0000000001;
            //    var equation = equations[singleVariableIndex];
            //    var variable = equation.Variables[0];
            //    var currentBestGuess = variable.SuggestedValue;
            //    for (; maxIterations > 0; maxIterations--)
            //    {
            //        var curValue = equation.Evaluate(v => currentBestGuess);
            //        if (Math.Abs(curValue) < threshold)
            //        {
            //            yield return new SolvedForVariable(singleVariableIndex, variable, currentBestGuess);
            //            yield break;
            //        }
            //        var curDerivative = equation.EvaluateDerivative(currentBestGuess);
            //        currentBestGuess -= curValue / curDerivative;
            //    }
            //}

            Array.Sort(equations, (e1, e2) =>
            {
                var c = e1.Degree.CompareTo(e2.Degree);
                if (c != 0) return c;
                c = e1.Variables.Length.CompareTo(e2.Variables.Length);
                if (c != 0) return c;
                c = (e1.Coefficients[0] != 0).CompareTo(e2.Coefficients[0] != 0);
                if (c != 0) return c;
                // TODO: Come up with some more comparisons
                return 0;
            });

            for (int eqIndex = 0; eqIndex < equations.Length; eqIndex++)
            {
                var eq = equations[eqIndex];
                var varDegrees = eq.GetVariableDegrees();
                var solveForIndex = Enumerable.Range(0, varDegrees.Length).MinElement(i => varDegrees[i]);
                var solveFor = eq.Variables[solveForIndex];

                if (varDegrees[solveForIndex] == 1)
                {
                    //   Any equation that is linear in one variable.
                    //   𝐄𝐱𝐚𝐦𝐩𝐥𝐞:
                    //   𝑥 𝑦 + 2 𝑥 + 3 𝑦 + 4 = 0
                    // ⇒ 𝑥 (𝑦 + 2) = −3 𝑦 − 4
                    // ⇒ 𝑥 = (−3 𝑦 − 4) / (𝑦 + 2)

                    var terms = eq.CollectTerms(solveForIndex);
                    yield return new SolvedForVariable(eqIndex, solveFor, Fraction.Create(-terms[0], terms[1]));

                    // If only one other variable, it’s pretty safe to say that we don’t need to explore other avenues
                    if (eq.Variables.Length < 3)
                        yield break;
                }
                //else if (varDegrees[solveForIndex] == 2)
                //{
                //    //   Quadratic polynomial
                //    //   𝑎 𝑥² + 𝑏 𝑥 + 𝑐 = 0
                //    // ⇒ 𝑥 = (−𝑏 ± √(𝑏² − 4 𝑎 𝑐)) / 2 𝑎

                //    var xTerms = eq.CollectTerms(solveForIndex);
                //    var aTerm = xTerms[2];
                //    var bTerm = xTerms[1];
                //    var cTerm = xTerms[0];   // 𝑐 can’t be 0 because if it were 0 this equation would have been handled earlier

                //    if (aTerm.Degree == 0 && bTerm == 0)
                //    {
                //        // ⇒ 𝑥 = ± √(−4 𝑎 𝑐) / 2 𝑎
                //        var a = aTerm.Coefficients[0];

                //        // Easier case: if 𝑐 is of the form −(𝑠 𝑦 ± 𝑡)² = −𝑠² 𝑦² ∓ 2 𝑠 𝑡 𝑦 − 𝑡², then 𝑥 = (𝑠 𝑦 ± 𝑡) / √𝑎 or 𝑥 = −(𝑠 𝑦 ± 𝑡) / √𝑎
                //        if (cTerm.Variables.Length == 1 && cTerm.Coefficients[0] <= 0 && cTerm.Coefficients[2] <= 0)
                //        {
                //            if (a < 0)
                //                yield break;
                //            var sqrta = Math.Sqrt(a);
                //            var s = Math.Sqrt(-cTerm.Coefficients[2]);
                //            var t = Math.Sqrt(-cTerm.Coefficients[0]);
                //            if (cTerm.Coefficients[1] == 2 * s * t)
                //            {
                //                var xSolution = s / sqrta * cTerm.Variables[0] - t / sqrta;
                //                yield return new SolvedForVariable(eqIndex, solveFor, xSolution);
                //                yield return new SolvedForVariable(eqIndex, solveFor, -xSolution);
                //                yield break;
                //            }
                //            else if (cTerm.Coefficients[1] == -2 * s * t)
                //            {
                //                var xSolution = s / sqrta * cTerm.Variables[0] + t / sqrta;
                //                yield return new SolvedForVariable(eqIndex, solveFor, xSolution);
                //                yield return new SolvedForVariable(eqIndex, solveFor, -xSolution);
                //                yield break;
                //            }
                //        }

                //        // Now 𝑐 cannot be a constant because that would make this a simple quadratic in 𝑥, which would have been handled earlier.
                //        // It also cannot be higher degree than 2. So it fits the pattern:
                //        // 𝑎 𝑥² + 𝑐 = 0
                //        // ⇒ 𝑥² = 𝑐 / (−𝑎)
                //        var xsq = cTerm / (-a);
                //        yield return new SolvedForVariable2(eqIndex, solveFor, new Root(xsq, false), new Root(xsq, true));
                //        yield break;
                //    }
                //    else if (aTerm.Degree == 0)
                //    {
                //        var surdTerm = bTerm.Square() - 4 * aTerm * cTerm;
                //        if (surdTerm.Degree == 2 && surdTerm.Variables.Length == 1 && surdTerm.Coefficients[0] >= 0 && surdTerm.Coefficients[2] >= 0)
                //        {
                //            var s = Math.Sqrt(surdTerm.Coefficients[2]);
                //            var t = Math.Sqrt(surdTerm.Coefficients[0]);
                //            Polynomial[] solArr;
                //            if (surdTerm.Coefficients[1] == 2 * s * t)
                //            {
                //                // ⇒ 𝑥 = (−𝑏 ± (𝑠 𝑦 + 𝑡)) / 2 𝑎
                //                solArr = Ut.NewArray(
                //                    (-bTerm + (s * surdTerm.Variables[0] + t)) / (2 * aTerm.Coefficients[0]),
                //                    (-bTerm - (s * surdTerm.Variables[0] + t)) / (2 * aTerm.Coefficients[0])
                //                );
                //            }
                //            else if (surdTerm.Coefficients[1] == -2 * s * t)
                //            {
                //                // ⇒ 𝑥 = (−𝑏 ± (𝑠 𝑦 − 𝑡)) / 2 𝑎
                //                solArr = Ut.NewArray(
                //                    (-bTerm + (s * surdTerm.Variables[0] - t)) / (2 * aTerm.Coefficients[0]),
                //                    (-bTerm - (s * surdTerm.Variables[0] - t)) / (2 * aTerm.Coefficients[0])
                //                );
                //            }
                //            else
                //            {
                //                continue;
                //            }
                //            foreach (var sol in solArr)
                //                yield return new SolvedForVariable(eqIndex, solveFor, sol);
                //            yield break;
                //        }
                //    }
                //}
            }

            //** BEGIN XL ALGORITHM

            var xlEquations = equations;

            for (int outerIteration = 0; outerIteration < 20; outerIteration++)
            {
                var degree = xlEquations.Max(eq => eq.Degree) + 2;
                var variables = xlEquations.SelectMany(eq => eq.Variables).Distinct().ToArray();
                var numCoeff = (degree + 1).Pow(variables.Length);

                // Populate ‘cells’
                var cells = new double[xlEquations.Length][];
                for (int xlIndex = 0; xlIndex < xlEquations.Length; xlIndex++)
                {
                    var eq = xlEquations[xlIndex];
                    var row = new double[numCoeff];
                    var varIndex = eq.Variables.Select(v => variables.IndexOf(v)).ToArray();
                    for (int i = 0; i < eq.Coefficients.Length; i++)
                    {
                        if (eq.Coefficients[i] == 0)
                            continue;
                        int index = 0, si = i;
                        for (int v = 0; v < eq.Variables.Length; v++)
                        {
                            var power = si % (eq.Degree + 1);
                            if (power != 0)
                                index += power * (degree + 1).Pow(varIndex[v]);
                            si /= (eq.Degree + 1);
                        }
                        row[index] = eq.Coefficients[i];
                    }
                    cells[xlIndex] = row;
                }

                // Calculate ‘totalDegree’ (e.g. 𝑥² 𝑦² has total degree 4) and ‘maxDegree’ (e.g. 𝑥² 𝑦² has max degree 2)
                var totalDegree = new int[numCoeff];
                var maxDegree = new int[numCoeff];
                for (int i = 0; i < numCoeff; i++)
                {
                    int si = i, maxDeg = 0, totalDeg = 0;
                    for (int v = 0; v < variables.Length; v++)
                    {
                        var power = si % (degree + 1);
                        maxDeg = Math.Max(maxDeg, power);
                        totalDeg += power;
                        si /= (degree + 1);
                    }
                    maxDegree[i] = maxDeg;
                    totalDegree[i] = totalDeg;
                }

                // Find a good ordering of the coefficients based on ‘totalDegree’ and ‘maxDegree’
                var sortKey = maxDegree;
                for (int i = 0; i < numCoeff; i++)
                    sortKey[i] += (degree + 1) * totalDegree[i];
                var coeffMap = Ut.NewArray(numCoeff, index => index);
                Array.Sort(sortKey, coeffMap);
                Array.Reverse(coeffMap);

                int col = 0;
                for (int row = 0, rows = cells.Length, innerIteration = 0; ; col++)
                {
                    var outputPath = @"D:\Daten\Upload\Geocos\XL-{0}-{1}.htm".Fmt(outerIteration, innerIteration);
                    var nextOutputFile = @"XL-{0}-{1}.htm".Fmt(outerIteration, innerIteration + 1);
                    Console.WriteLine("Writing: {0}".Fmt(outputPath));
                    GeoUt.WriteHtml(
                        @"
                            <ul>{0}</ul>
                            <p><a href='{1}'>Next</a></p>
                        ".Fmt(
                            cells.Select(rw => "<li>$ {0} = 0 $</li>".Fmt(new Polynomial(degree, variables, rw))).JoinString(),
                            nextOutputFile
                        ),
                        outputPath);
                    GeoUt.OutputTable(
                        coeffMap.Select(ci => "$ {0} $".Fmt(new Polynomial(degree, variables, Ut.NewArray(numCoeff, ci2 => ci == ci2 ? 1d : 0d)))).ToArray(),
                        //cells.Select(rw => "$ {0} = 0 $".Fmt(new Polynomial(degree, variables, rw))).ToArray(),
                        cells.Select(rw => "").ToArray(),
                        cells.Select(rw => rw.Select((d, i) => rw[coeffMap[i]].ToString("#.##")).ToArray()).ToArray(),
                        null,
                        @"D:\Daten\Upload\Geocos\XL-{0}-{1}-table.htm".Fmt(outerIteration, innerIteration)
                    );

                    Ut.Assert(col < numCoeff);
                    if (row >= rows)
                        break;

                    var equationToUse = Enumerable.Range(row, rows - row).FirstOrDefault(rw => cells[rw][coeffMap[col]] != 0, -1);
                    if (equationToUse == -1)
                        continue;

                    if (equationToUse != row)
                    {
                        var tmp = cells[equationToUse];
                        cells[equationToUse] = cells[row];
                        cells[row] = tmp;
                    }

                    var divideBy = cells[row][coeffMap[col]];
                    if (divideBy != 1)
                        for (int cl = col; cl < numCoeff; cl++)
                            cells[row][coeffMap[cl]] /= divideBy;

                    for (int r = row + 1; r < rows; r++)
                    {
                        var multiplier = cells[r][coeffMap[col]];
                        if (multiplier == 0)
                            continue;
                        for (int cl = col; cl < numCoeff; cl++)
                            cells[r][coeffMap[cl]] -= cells[row][coeffMap[cl]] * multiplier;
                    }

                    innerIteration++;
                    row++;
                }

                if (totalDegree[coeffMap[col]] <= 2)
                {
                    yield return new NewEquations(
                        Enumerable.Range(cells.Length - equations.Length, equations.Length).Select(
                            ri => new Polynomial(degree, variables, cells[ri]).Simplify()
                        ).Reverse().ToArray()
                    );
                    yield break;
                }

                xlEquations = xlEquations
                    .Concat(variables.SelectMany(v => xlEquations.Select(equ => equ * v))).ToArray();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            System.Diagnostics.Debugger.Break();
            throw new NotImplementedException();
        }
    }
}
