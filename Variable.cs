using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    sealed class Variable
    {
        public string Name { get; private set; }
        public double SuggestedValue { get; set; }
        public bool NonNegative { get; set; }

        public Variable(string name, bool nonNegative = false)
        {
            Name = name;
            NonNegative = nonNegative;
        }

        public override string ToString() { return Name; }

        public static Polynomial operator +(Variable a, Variable b) { return (Polynomial) a + (Polynomial) b; }
        public static Polynomial operator +(Variable a, double b) { return (Polynomial) a + b; }
        public static Polynomial operator +(double a, Variable b) { return (Polynomial) b + a; }

        public static Polynomial operator -(Variable a, Variable b) { return (Polynomial) a + (-(Polynomial) b); }
        public static Polynomial operator -(Variable a, double b) { return (Polynomial) a + (-b); }
        public static Polynomial operator -(double a, Variable b) { return (-(Polynomial) b) + a; }

        public static Polynomial operator *(double a, Variable b) { return a * (Polynomial) b; }
        public static Polynomial operator *(Variable a, double b) { return b * (Polynomial) a; }
        public static Polynomial operator *(Variable a, Variable b) { return (Polynomial) a * (Polynomial) b; }

        public Polynomial Square() { return ((Polynomial) this).Square(); }
        public Polynomial Pow(int b) { return b == 0 ? 1 : ((Polynomial) this).Pow(b); }
    }
}
