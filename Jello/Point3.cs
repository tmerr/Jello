using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello
{
    struct Point3
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Point3(int x, int y, int z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
