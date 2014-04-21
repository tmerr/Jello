using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Physics
{
    class StaticAnchor : IAnchor
    {
        public StaticAnchor(Vector3 position)
        {
            AnchorPosition = position;
        }

        public Vector3 AnchorPosition { get; private set; }

        public Vector3 AnchorVelocity
        {
            get { return Vector3.Zero; }
        }
    }
}
