using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Physics
{
    /// <summary>
    /// An immutable node at some point in time.
    /// </summary>
    struct Node
    {
        public readonly IAnchor Anchor;
        public readonly float Mass;
        public readonly Vector3 Position;
        public readonly Vector3 Velocity;

        public Node(IAnchor anchor, float mass, Vector3 position, Vector3 velocity)
        {
            Anchor = anchor;
            Mass = mass;
            Position = position;
            Velocity = velocity;
        }
    }
}
