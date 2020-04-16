using System;
using RT.Util;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class Polynomial : Formula
    {
        public int Degree { get; private set; }
        public Variable[] Variables { get; private set; }
        public double[] Coefficients { get; private set; }

        public Polynomial(int degree, Variable[] variables, params double[] coefficients)
        {
            Degree = degree;
            Variables = variables;
            Coefficients = coefficients;
        }

        public static implicit operator Polynomial(Variable variable)
        {
            return new Polynomial(1, new[] { variable }, 0, 1);
        }

        public static implicit operator Polynomial(double value)
        {
            return new Polynomial(0, new Variable[] { }, value);
        }

        public static bool operator ==(Polynomial eq, double value) { return eq.Degree == 0 && eq.Coefficients[0] == value; }
        public static bool operator !=(Polynomial eq, double value) { return !(eq == value); }

        public override int GetHashCode()
        {
            var h = Degree;
            if (Variables != null)
            {
                h = 3 * h + Variables.Length;
                for (int i = 0; i < Variables.Length; i++)
                    h = 37 * h + Variables[i].GetHashCode();
            }
            if (Coefficients != null)
            {
                h = 5 * h + Coefficients.Length;
                for (int i = 0; i < Coefficients.Length; i++)
                    h = 101 * h + Coefficients[i].GetHashCode();
            }
            return h;
        }

        public override bool Equals(object obj)
        {
            return obj is Polynomial && ((Polynomial) obj) == this;
        }

        public static Polynomial operator +(Polynomial one, Polynomial two)
        {
            var degree = Math.Max(one.Degree, two.Degree);
            var variables = one.Variables.Concat(two.Variables).Distinct().ToArray();
            var coefficients = new double[(degree + 1).Pow(variables.Length)];

            foreach (var eq in new[] { one, two })
            {
                var varIndex = eq.Variables.Select(v => variables.IndexOf(v)).ToArray();
                for (int i = 0; i < eq.Coefficients.Length; i++)
                {
                    if (eq.Coefficients[i] == 0)
                        continue;
                    var index = 0;
                    var si = i;
                    for (int v = 0; v < eq.Variables.Length; v++)
                    {
                        var power = si % (eq.Degree + 1);
                        if (power != 0)
                            index += power * (degree + 1).Pow(varIndex[v]);
                        si /= (eq.Degree + 1);
                    }
                    coefficients[index] += eq.Coefficients[i];
                }
            }

            return new Polynomial(degree, variables, coefficients).Simplify();
        }

        public static Polynomial operator +(Polynomial one, double two)
        {
            return new Polynomial(one.Degree, one.Variables, Ut.NewArray(one.Coefficients.Length, index => one.Coefficients[index] + (index == 0 ? two : 0)));
        }

        public static Polynomial operator +(double one, Polynomial two) { return two + one; }
        public static Polynomial operator -(Polynomial one, double two) { return one + (-two); }
        public static Polynomial operator -(double one, Polynomial two) { return (-two) + one; }

        public static Polynomial operator -(Polynomial source)
        {
            return new Polynomial(source.Degree, source.Variables, source.Coefficients.Select(c => -c).ToArray());
        }

        public static Polynomial operator -(Polynomial one, Polynomial two)
        {
            return one + (-two);
        }

        public static Polynomial operator *(double one, Polynomial two)
        {
            if (one == 0)
                return 0;
            if (one == 1)
                return two;
            return new Polynomial(two.Degree, two.Variables, two.Coefficients.Select(c => one * c).ToArray());
        }

        public static Polynomial operator *(Polynomial one, double two) { return two * one; }
        public static Polynomial operator /(Polynomial one, double two) { return (1 / two) * one; }

        public static Polynomial operator *(Polynomial one, Polynomial two)
        {
            if (one == 0) return one;
            if (two == 0) return two;
            if (one == 1) return two;
            if (two == 1) return one;

            // STEP 1: Determine what degree the resulting equation will have
            var degree = Math.Max(one.Degree, two.Degree);
            for (int v1 = 0; v1 < one.Variables.Length; v1++)
            {
                var variable = one.Variables[v1];
                var v2 = two.Variables.IndexOf(variable);
                if (v2 == -1)
                    continue;

                var maxPower1 = 0;
                var mult1 = (one.Degree + 1).Pow(v1);
                for (int i = 0; i < one.Coefficients.Length; i++)
                    if (one.Coefficients[i] != 0)
                        maxPower1 = Math.Max(maxPower1, (i / mult1) % (one.Degree + 1));

                var maxPower2 = 0;
                var mult2 = (two.Degree + 1).Pow(v2);
                for (int i = 0; i < two.Coefficients.Length; i++)
                    if (two.Coefficients[i] != 0)
                        maxPower2 = Math.Max(maxPower2, (i / mult2) % (two.Degree + 1));

                degree = Math.Max(degree, maxPower1 + maxPower2);
            }

            // STEP 2: Construct the new equation
            var variables = one.Variables.Concat(two.Variables).Distinct().ToArray();
            var coefficients = new double[(degree + 1).Pow(variables.Length)];
            var powers1 = new int[one.Variables.Length];
            for (int c1 = 0; c1 < one.Coefficients.Length; c1++)
            {
                if (one.Coefficients[c1] != 0)
                {
                    var powers2 = new int[two.Variables.Length];
                    for (int c2 = 0; c2 < two.Coefficients.Length; c2++)
                    {
                        if (two.Coefficients[c2] != 0)
                        {
                            var index = 0;
                            var mult = 1;
                            for (int v = 0; v < variables.Length; v++)
                            {
                                var v1index = one.Variables.IndexOf(variables[v]);
                                var v2index = two.Variables.IndexOf(variables[v]);
                                index += mult * ((v1index == -1 ? 0 : powers1[v1index]) + (v2index == -1 ? 0 : powers2[v2index]));
                                mult *= (degree + 1);
                            }
                            coefficients[index] += one.Coefficients[c1] * two.Coefficients[c2];
                        }

                        var index2 = powers2.IndexOf(j => j < two.Degree);
                        if (index2 != -1)
                        {
                            powers2[index2]++;
                            for (int j = 0; j < index2; j++)
                                powers2[j] = 0;
                        }
                    }
                }

                var index1 = powers1.IndexOf(i => i < one.Degree);
                if (index1 != -1)
                {
                    powers1[index1]++;
                    for (int i = 0; i < index1; i++)
                        powers1[i] = 0;
                }
            }

            // We already determined the required degree, so no Simplify necessary
            return new Polynomial(degree, variables, coefficients);
        }

        public Polynomial Square()
        {
            var degree = Degree * 2;
            var coefficients = new double[(degree + 1).Pow(Variables.Length)];
            for (int i = 0; i < Coefficients.Length; i++)
                for (int j = 0; j < Coefficients.Length; j++)
                {
                    var index = 0;
                    var mult = 1;
                    var si = i;
                    var sj = j;
                    for (int v = 0; v < Variables.Length; v++)
                    {
                        index += mult * ((si % (Degree + 1)) + (sj % (Degree + 1)));
                        mult *= (degree + 1);
                        si /= (Degree + 1);
                        sj /= (Degree + 1);
                    }
                    coefficients[index] += Coefficients[i] * Coefficients[j];
                }

            // Simplify would not achieve anything here
            return new Polynomial(degree, Variables, coefficients);
        }

        public Polynomial Pow(int b)
        {
            if (b < 0)
                throw new ArgumentOutOfRangeException("b", "'b' must be non-negative.");

            // We could write “result = 1” and it would save some typing in the rest of the algorithm; however,
            // it would be slower because of an extra object instantiation and an extra multiplication operation.
            Polynomial result = null, a = this;
            while (b > 0)
            {
                if ((b & 1) != 0)
                    result = result == null ? a : result * a;
                a = a.Square();
                b >>= 1;
            }
            return result ?? 1;
        }

        public Polynomial Simplify()
        {
            if (Degree == 0)
                return this;
            if (Variables.Length == 0)
                return new Polynomial(0, Variables, Coefficients);

            // Determine what degree the resulting equation will have
            var varDegrees = GetVariableDegrees();
            var degree = varDegrees.Max();
            var variables = Enumerable.Range(0, Variables.Length).Where(i => varDegrees[i] > 0).Select(i => Variables[i]).ToArray();
            if (degree >= Degree && variables.Length >= Variables.Length)
                return this;

            var coefficients = new double[(degree + 1).Pow(variables.Length)];
            coefficients[0] = Coefficients[0];
            for (int iOld = 1; iOld < Coefficients.Length; iOld++)
                if (Coefficients[iOld] != 0)
                {
                    var si = iOld;
                    var iNew = 0;
                    var mult = 1;
                    for (int v = 0; v < Variables.Length; v++)
                    {
                        if (varDegrees[v] > 0)
                        {
                            iNew += mult * (si % (Degree + 1));
                            mult *= (degree + 1);
                        }
                        si /= (Degree + 1);
                    }
                    coefficients[iNew] = Coefficients[iOld];
                }
            return new Polynomial(degree, variables, coefficients);
        }

        public int[] GetVariableDegrees()
        {
            var varDegrees = new int[Variables.Length];
            for (int i = 1; i < Coefficients.Length; i++)
                if (Coefficients[i] != 0)
                {
                    var si = i;
                    for (int v = 0; v < Variables.Length; v++)
                    {
                        varDegrees[v] = Math.Max(varDegrees[v], si % (Degree + 1));
                        si /= (Degree + 1);
                    }
                }
            return varDegrees;
        }

        public int GetVariableDegree(int variableIndex)
        {
            var degree = 0;
            var mult = (Degree + 1).Pow(variableIndex);
            for (int i = 1; i < Coefficients.Length; i++)
                if (Coefficients[i] != 0)
                    degree = Math.Max(degree, (i / mult) % (Degree + 1));
            return degree;
        }

        public int GetLowestDegreeVariableIndex()
        {
            var degrees = GetVariableDegrees();
            return Enumerable.Range(0, degrees.Length).MinElement(vi => degrees[vi]);
        }

        public Tuple<Polynomial, Variable> Decimate()
        {
            // This time, we want not the *highest* but the *lowest* degree in which a variable appears.
            // For example, in an equation like
            //      𝑥² 𝑦 + 2 𝑥 = 0
            // 𝑥 has the minimum degree 1 (and 𝑦 has 0).
            // The following array ‘varMinDegrees’ will contain the number Degree − 𝑚, where 𝑚 is the
            // lowest degree found so far. This way, we can start with an array initialized to zeroes (which
            // means Degree).
            var varMinDegrees = new int[Variables.Length];
            for (int i = 1; i < Coefficients.Length; i++)
                if (Coefficients[i] != 0)
                {
                    var si = i;
                    for (int v = 0; v < Variables.Length; v++)
                    {
                        varMinDegrees[v] = Math.Max(varMinDegrees[v], Degree - si % (Degree + 1));
                        si /= (Degree + 1);
                    }
                }

            // Find the variable with the highest minimum degree, and its actual minimum degree
            var varIndex = Enumerable.Range(0, varMinDegrees.Length).MinElement(i => varMinDegrees[i]);
            var degree = Degree - varMinDegrees[varIndex];
            if (degree == 0)
                return null;

            var mult = (Degree + 1).Pow(varIndex);
            var newCoefficients = Ut.NewArray(Coefficients.Length, i =>
            {
                var otherIndex = (i / mult + degree) * mult + (i % mult);
                return otherIndex >= Coefficients.Length ? 0 : Coefficients[otherIndex];
            });
            return Tuple.Create(new Polynomial(Degree, Variables, newCoefficients).Simplify(), Variables[varIndex]);
        }

        public Polynomial[] CollectTerms(int varIndex)
        {
            // For example, if the polynomial is
            //   𝑎 𝑥² 𝑦² + 𝑏 𝑥 𝑦² + 𝑐 𝑦² + 𝑑 𝑥² 𝑦 + 𝑒 𝑥 𝑦 + 𝑓 𝑦 + 𝑔 𝑥² + 𝑗 𝑥 + 𝑘
            // and ‘varIndex’ is the index of variable 𝑥, collect the terms with equal powers of 𝑥 in them, yielding:
            //   (𝑎 𝑦² + 𝑑 𝑦 + 𝑔) 𝑥² + (𝑏 𝑦² + 𝑒 𝑦 + 𝑗) 𝑥 + (𝑐 𝑦² + 𝑓 𝑦 + 𝑘)
            // and thus, the polynomials returned are:
            //  • 𝑐 𝑦² + 𝑓 𝑦 + 𝑘
            //  • 𝑏 𝑦² + 𝑒 𝑦 + 𝑗
            //  • 𝑎 𝑦² + 𝑑 𝑦 + 𝑔

            var coefficientses = Ut.NewArray<double[]>(Degree + 1, i => new double[(Degree + 1).Pow(Variables.Length - 1)]);
            var xMult = (Degree + 1).Pow(varIndex);
            for (int i = 0; i < Coefficients.Length; i++)
                if (Coefficients[i] != 0)
                    coefficientses[(i / xMult) % (Degree + 1)][(i % xMult) + (i / xMult / (Degree + 1)) * xMult] = Coefficients[i];
            var varsWithoutCollected = Variables.Where((v, i) => i != varIndex).ToArray();
            return coefficientses.Select(coeff => new Polynomial(Degree, varsWithoutCollected, coeff).Simplify()).ToArray();
        }

        /// <summary>
        ///     Evaluates the current equation.</summary>
        /// <param name="func">
        ///     If <c>null</c>, all variables are evaluated according to their suggested values (<see
        ///     cref="Variable.SuggestedValue"/>). Otherwise, this function is invoked to retrieve the value for each
        ///     variable.</param>
        public override double Evaluate(Func<Variable, double> func = null)
        {
            var variableValues = Variables.Select(v => func == null ? v.SuggestedValue : func(v)).ToArray();

            var result = 0.0;

            for (int i = 0; i < Coefficients.Length; i++)
                if (Coefficients[i] != 0)
                {
                    var term = Coefficients[i];
                    var si = i;
                    for (int v = 0; v < Variables.Length; v++)
                    {
                        var pow = (si % (Degree + 1));
                        if (pow != 0)
                            term *= pow == 1 ? variableValues[v] : Math.Pow(variableValues[v], pow);
                        si /= (Degree + 1);
                    }
                    result += term;
                }

            return result;
        }

        public override Formula Substitute(Variable variable, Formula value)
        {
            if (!Variables.Contains(variable))
                return this;
            if (value is Polynomial)
                return SubstituteAsEquation(variable, (Polynomial) value);
            if (Variables.Length == 1 && Degree == 1 && Coefficients[0] == 0 && Coefficients[1] == 1)
                return value;

#warning TODO
            return this;
        }

        public Polynomial SubstituteAsEquation(Variable variable, Formula value)
        {
            var vIndex = Variables.IndexOf(variable);
            if (vIndex == -1)
                return this;
            var vDegree = GetVariableDegree(vIndex);

            Polynomial valuePolynomial;
            Fraction valueFraction;
            Polynomial numer, denom;

            if ((valuePolynomial = value as Polynomial) != null)
            {
                return CollectTerms(vIndex)
                    .Select((term, pow) => term == 0 ? term : term * valuePolynomial.Pow(pow))
                    .Aggregate((prev, next) => prev + next);
            }
            else if ((valueFraction = value as Fraction) != null &&
                (numer = valueFraction.Numerator as Polynomial) != null &&
                (denom = valueFraction.Denominator as Polynomial) != null)
            {
                return CollectTerms(vIndex)
                    .Select((term, pow) => term == 0 ? term : term * numer.Pow(pow) * denom.Pow(vDegree - pow))
                    .Aggregate((prev, next) => prev + next);
            }
            else
                throw new NotImplementedException();
        }

        public string CoefficientVariablesAsString(int coefficientIndex)
        {
            var variables = new List<string>();
            var si = coefficientIndex;
            for (int v = 0; v < Variables.Length; v++)
            {
                var power = si % (Degree + 1);
                if (power != 0)
                    variables.Add(Variables[v].Name + (power == 1 ? "" : "^" + power));
                si /= (Degree + 1);
            }
            variables.Sort();
            //variables.Reverse();
            return variables.JoinString(" ");
        }

        public Polynomial CoefficientVariablesAsPolynomial(int coefficientIndex)
        {
            Polynomial variables = 1;
            var si = coefficientIndex;
            for (int v = 0; v < Variables.Length; v++)
            {
                var power = si % (Degree + 1);
                if (power != 0)
                    variables *= Variables[v].Pow(power);
                si /= (Degree + 1);
            }
            return variables;
        }

        public static Polynomial CoefficientVariablesAsPolynomial(int degree, Variable[] variables, int coefficientIndex)
        {
            Polynomial result = 1;
            var si = coefficientIndex;
            for (int v = 0; v < variables.Length; v++)
            {
                result *= variables[v].Pow(si % (degree + 1));
                si /= (degree + 1);
            }
            return result;
        }

        public override string ToString()
        {
            var stuff = new List<string>();
            for (int i = Coefficients.Length - 1; i >= 0; i--)
            {
                if (Coefficients[i] == 0)
                    continue;
                if (Coefficients[i] < 0)
                    stuff.Add("-");
                else if (stuff.Count > 0)
                    stuff.Add("+");
                var abs = Math.Abs(Coefficients[i]);
                if (abs != 1 || i == 0)
                    stuff.Add(abs.ToString("#.##################"));

                var si = i;
                for (int v = 0; v < Variables.Length; v++)
                {
                    var power = si % (Degree + 1);
                    if (power != 0)
                        stuff.Add(Variables[v].Name + (power == 1 ? "" : "^" + power));
                    si /= (Degree + 1);
                }
            }
            return stuff.Count == 0 ? "0" : stuff.JoinString(" ");
        }

        public override string ToMaple()
        {
            var stuff = new List<string>();
            for (int i = Coefficients.Length - 1; i >= 0; i--)
            {
                if (Coefficients[i] == 0)
                    continue;
                if (Coefficients[i] < 0)
                    stuff.Add("-");
                else if (stuff.Count > 0)
                    stuff.Add("+");

                var thisTerm = new List<string>();
                var abs = Math.Abs(Coefficients[i]);
                if (abs != 1 || i == 0)
                    thisTerm.Add(abs.ToString());

                var si = i;
                for (int v = 0; v < Variables.Length; v++)
                {
                    var power = si % (Degree + 1);
                    if (power != 0)
                        thisTerm.Add(Variables[v].Name.Replace("{", "").Replace("}", "") + (power == 1 ? "" : "^" + power));
                    si /= (Degree + 1);
                }

                if (thisTerm.Any())
                    stuff.Add(thisTerm.JoinString("*"));
            }
            return stuff.Count == 0 ? "0" : stuff.JoinString(" ");
        }

        public double EvaluateDerivative(double value)
        {
            if (Variables.Length != 1)
                throw new InvalidOperationException("EvaluateDerivative() is only allowed on polynomials of only one variable.");
            double result = 0;
            for (int i = 1; i < Coefficients.Length; i++)
                result += Coefficients[i] * i * Math.Pow(value, i - 1);
            return result;
        }
    }
}
