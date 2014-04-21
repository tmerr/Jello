using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Physics
{
    /// <summary>
    /// An immutable spring at some point in time.
    /// </summary>
    struct Spring
    {
        public readonly int A;
        public readonly int B;
        public readonly float RestLength;
        public readonly float SpringConstant;
        public readonly float DampingConstant;

        public Spring(int a, int b, float restLength, float springConstant, float dampingConstant)
        {
            A = a;
            B = b;
            RestLength = restLength;
            SpringConstant = springConstant;
            DampingConstant = dampingConstant;
        }
    }
}
