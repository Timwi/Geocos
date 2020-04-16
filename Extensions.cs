using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocos
{
    static class Extensions
    {
        public static int Pow(this int a, int b)
        {
            checked
            {
                if (b < 0)
                    throw new ArgumentOutOfRangeException("b", "'b' must be non-negative.");
                if (b == 0)
                    return 1;
                var result = 1;
                while (true)
                {
                    if ((b & 1) != 0)
                        result *= a;
                    if (b == 1)
                        return result;
                    a *= a;
                    b >>= 1;
                }
            }
        }
    }
}
