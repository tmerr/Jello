using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Entities.Physics
{
    class Spring
    {
        public INode A;
        public INode B;
        public float RestLength;
        private float SpringConstant; // in N/m
        private float DampeningConstant;

        public Spring(INode a, INode b, float restLength, float springConstant, float dampeningConstant)
        {
            A = a;
            B = b;
            RestLength = restLength;
            SpringConstant = springConstant;
            DampeningConstant = dampeningConstant;
        }

        public void ApplyForceToNodes(float deltaTime)
        {
            var Lvect = A.Position - B.Position;
            Vector3 F1 = -SpringConstant * (Lvect.Length - RestLength) * Lvect.Normalized();
            Vector3 F2 = Vector3.Zero;
            if (DampeningConstant != 0)
            {
                F2 = -DampeningConstant * (Vector3.Dot((A.Velocity - B.Velocity), Lvect) / Lvect.Length) * Lvect.Normalized();
            }
            var FSum = F1 + F2;
            A.ApplyForce(FSum, deltaTime);
            B.ApplyForce(-FSum, deltaTime);
        }
    }
}
