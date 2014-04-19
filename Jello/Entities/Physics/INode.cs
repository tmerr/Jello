using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Entities.Physics
{
    interface INode
    {
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
        float Mass { get; set; }

        void ApplyForce(Vector3 force, float deltaTime);

        void ApplyAcceleration(Vector3 force, float deltaTime);

        void CrunchVelocities(float deltaTime);
    }
}
