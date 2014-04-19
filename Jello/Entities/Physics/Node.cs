using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Entities.Physics
{
    class Node : INode
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }

        public Node(Vector3 position, float mass)
        {
            Position = position;
            Mass = mass;
            Velocity = Vector3.Zero;
        }

        public void ApplyForce(Vector3 force, float deltaTime)
        {
            Velocity += (force / Mass) * deltaTime;
        }

        public void ApplyAcceleration(Vector3 accel, float deltaTime)
        {
            Velocity += accel * deltaTime;
        }

        public void CrunchVelocities(float deltaTime)
        {
            Position += Velocity * deltaTime;
        }
    }
}
