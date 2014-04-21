using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Entities.Physics
{
    class MassSpringSystem
    {
        private List<Node> _nodes = new List<Node>();
        private List<Spring> _springs = new List<Spring>();
        private Func<Node, float, Vector3> _externalAcceleration = (node, dt) => Vector3.Zero;
        private IntegratorType _integrator;

        public MassSpringSystem(IntegratorType integrator)
        {
            _integrator = integrator;
        }

        public void AddNode(Vector3 position, float mass)
        {
            _nodes.Add(new Node(null, mass, position, Vector3.Zero));
        }

        /// <summary>Add a node whose position, velocity and mass are fixed to the anchor.</summary>
        public void AddAnchor(IAnchor anchor)
        {
            _nodes.Add(new Node(anchor, -1, anchor.AnchorPosition, anchor.AnchorVelocity));
        }

        public void AddSpring(int nodeA, int nodeB, float restLength, float springConstant, float dampingConstant)
        {
            _springs.Add(new Spring(nodeA, nodeB, restLength, springConstant, dampingConstant));
        }

        public void Step(float dt, float minimumTime)
        {
            int steps = (int)Math.Ceiling(dt / minimumTime);
            float stepSize = dt / steps;
            for (int i = 0; i < steps; i++)
            {
                if (_integrator == IntegratorType.RungeKutta4)
                    _nodes = RK4(_nodes, _springs, _externalAcceleration, stepSize);
                else if (_integrator == IntegratorType.EulerMethod)
                    _nodes = EulerMethod(_nodes, _springs, _externalAcceleration, stepSize);
            }
            _externalAcceleration = (node, t) => Vector3.Zero;
        }

        public List<Vector3> GetPositions()
        {
            return _nodes.Select(n => n.Position).ToList();
        }

        public void SetExternalAcceleration(Func<Node, float, Vector3> func)
        {
            _externalAcceleration = func;
        }

        private static List<Vector3> AccelerationFunction(List<Node> nodes, List<Spring> springs, Func<Node, float, Vector3> externalAcceleration, float dt)
        {
            var accelerations = new List<Vector3>(new Vector3[nodes.Count]);
            foreach (var spring in springs)
            {
                var A = nodes[spring.A];
                var B = nodes[spring.B];

                var Lvect = A.Position - B.Position;
                var hooke = -spring.SpringConstant * (Lvect.Length - spring.RestLength) * Lvect.Normalized();
                var damping = -spring.DampingConstant * (Vector3.Dot(A.Velocity - B.Velocity, Lvect) / Lvect.Length) * (Lvect / Lvect.Length);
                var force = hooke + damping;
                
                if (A.Anchor == null)
                    accelerations[spring.A] += force / A.Mass;
                if (B.Anchor == null)
                    accelerations[spring.B] -= force / A.Mass;
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Anchor == null)
                    accelerations[i] += externalAcceleration(nodes[i], dt);
            }

            return accelerations;
        }

        private static Node TransitionNode(Node previous, Vector3 newPosition, Vector3 newVelocity)
        {
            if (previous.Anchor == null)
                return new Node(null, previous.Mass, newPosition, newVelocity);
            else
                return new Node(previous.Anchor, previous.Mass, previous.Anchor.AnchorPosition, previous.Anchor.AnchorVelocity);
        }

        private static List<Node> RK4(List<Node> nodes, List<Spring> springs, Func<Node, float, Vector3> externalAcceleration, float dt)
        {
            var n1 = nodes;
            var a1 = AccelerationFunction(nodes, springs, externalAcceleration, 0);
            
            var n2 = n1.Zip(a1, (node, accel) => TransitionNode(node, node.Position + (dt/2) * node.Velocity, node.Velocity + (dt/2) * accel)).ToList();
            var a2 = AccelerationFunction(n2, springs, externalAcceleration, dt/2);

            var n3 = n2.Zip(a2, (node, accel) => TransitionNode(node, node.Position + (dt/2) * node.Velocity, node.Velocity + (dt/2) * accel)).ToList();
            var a3 = AccelerationFunction(n3, springs, externalAcceleration, dt/2);

            var n4 = n3.Zip(a3, (node, accel) => TransitionNode(node, node.Position + dt * node.Velocity, node.Velocity + dt * accel)).ToList();
            var a4 = AccelerationFunction(n4, springs, externalAcceleration, dt);

            var result = nodes.Select((node, i) =>
                TransitionNode(node,
                               node.Position + dt * (n1[i].Velocity + 2 * n2[i].Velocity + 2 * n3[i].Velocity + n4[i].Velocity) / 6,
                               node.Velocity + dt * (a1[i] + 2 * a2[i] + 2 * a3[i] + a4[i]) / 6));

            return result.ToList();
        }

        private static List<Node> EulerMethod(List<Node> nodes, List<Spring> springs, Func<Node, float, Vector3> externalAcceleration, float dt)
        {
            var accels = AccelerationFunction(nodes, springs, externalAcceleration, dt);

            var result = nodes.Select((node, i) =>
                TransitionNode(node,
                               node.Position + dt * node.Velocity,
                               node.Velocity + dt * accels[i]));

            return result.ToList();
        }
    }
}
