using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Entities.Physics
{
    /// <summary>
    /// Ignores everything but position
    /// </summary>
    class AnchorNode : INode
    {
        public OpenTK.Vector3 Position { get; set; }

        public OpenTK.Vector3 Velocity { get; set; }

        public float Mass { get; set; }

        public AnchorNode(Vector3 position)
        {
            Position = position;
        }

        public void ApplyForce(Vector3 force, float deltaTime)
        {
            // nothing :D
        }

        public void ApplyAcceleration(Vector3 force, float deltaTime)
        {
            // nothing :D
        }

        public void CrunchVelocities(float deltaTime)
        {
            // nothing :D
        }
    }
}
