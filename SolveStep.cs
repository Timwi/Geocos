using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geocos
{
    abstract class SolveStep
    {
        public int? EquationIndex { get; private set; }
        public SolveStep(int? equationIndex) { EquationIndex = equationIndex; }
        public abstract override string ToString();
    }

    class SolvedForVariable : SolveStep
    {
        public Variable SolvedFor { get; private set; }
        public Formula Solution { get; private set; }

        public SolvedForVariable(int equationIndex, Variable solvedFor, Formula solution)
            : base(equationIndex)
        {
            SolvedFor = solvedFor;
            Solution = solution;
        }

        public override string ToString()
        {
            return "{0} = {1}".Fmt(SolvedFor, Solution);
        }
    }

    sealed class SolvedForVariable2 : SolvedForVariable
    {
        public Formula Solution2 { get; private set; }

        public SolvedForVariable2(int equationIndex, Variable solvedFor, Formula solution1, Formula solution2)
            : base(equationIndex, solvedFor, solution1)
        {
            Solution2 = solution2;
        }

        public override string ToString()
        {
            return "{0} ⇒ {1} = {2} or {3}".Fmt(EquationIndex, SolvedFor, Solution, Solution2);
        }
    }

    sealed class RedundantEquation : SolveStep
    {
        public RedundantEquation(int equationIndex)
            : base(equationIndex) { }
        public override string ToString() { return "Equation #{0} is redundant.".Fmt(EquationIndex); }
    }

    sealed class NoSolution : SolveStep
    {
        public NoSolution(int equationIndex)
            : base(equationIndex) { }
        public override string ToString() { return "Equation #{0} has no solution.".Fmt(EquationIndex); }
    }

    sealed class EquationSimplified : SolveStep
    {
        public Polynomial SimplerEquation { get; private set; }
        public EquationSimplified(int equationIndex, Polynomial simplerEquation)
            : base(equationIndex)
        {
            SimplerEquation = simplerEquation;
        }
        public override string ToString() { return "Simplifying equation #{0} to {1}.".Fmt(EquationIndex, SimplerEquation); }
    }

    sealed class NewEquations : SolveStep
    {
        public Polynomial[] Equations { get; private set; }
        public NewEquations(Polynomial[] equations) : base(null) { Equations = equations; }
        public override string ToString()
        {
            return "New equations: {{ {0} }}".Fmt(Equations.JoinString("; "));
        }
    }
}
