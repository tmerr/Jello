using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jello.Physics;
using OpenTK;

namespace Jello.Entities
{
    class Cloth
    {
        private VertexBuffer<Vector3> _vertexBuffer;
        private VertexBuffer<Vector3> _normalsBuffer;
        private VertexBuffer<uint> _indicesBuffer;

        MassSpringSystem _system = new MassSpringSystem(IntegratorType.RungeKutta4);

        private const float Gravity = 9.8f;
        private const float SpringConstant = 100f;
        private const float DampingConstant = 1.5f;
        private const float NodeMass = 0.03f; // in kg
        private float MinimumTimeStep = 0.01f;

        private Func<Node, Vector3> _pushAcceleration = n => Vector3.Zero;

        public ModelData ModelData
        {
            get
            {
                return new ModelData(_vertexBuffer, _normalsBuffer, _indicesBuffer);
            }
        }

        public Cloth()
        {
            _vertexBuffer = new VertexBuffer<Vector3>();
            _normalsBuffer = new VertexBuffer<Vector3>();
            _indicesBuffer = new VertexBuffer<uint>();

            ArrangeNodes(new Vector3(0, 0, 2), new Vector2(5, 5), 10);
        }

        private void ArrangeNodes(Vector3 bottomLeft, Vector2 size, int nodesPerAxis)
        {
            float spacingX = size.X / nodesPerAxis;
            float spacingY = size.Y / nodesPerAxis;
            float spacingDiag = new Vector2(spacingX, spacingY).Length;

            // Create nodes
            for (int y = 0; y < nodesPerAxis - 1; y++)
                for (int x = 0; x < nodesPerAxis; x++)
                    _system.AddNode(bottomLeft + new Vector3(x * spacingX, y * spacingY, 0f), NodeMass);

            for (int x = 0; x < nodesPerAxis; x++)
                _system.AddAnchor(new StaticAnchor(bottomLeft + new Vector3(x * spacingX, (nodesPerAxis - 1) * spacingY, 0f)));

            // Create springs
            for (int y = 0; y < nodesPerAxis; y++)
            {
                for (int x = 0; x < nodesPerAxis; x++)
                {
                    int current = y * nodesPerAxis + x;
                    int right = y * nodesPerAxis + x + 1;
                    int down = (y + 1) * nodesPerAxis + x;
                    int downRight = (y + 1) * nodesPerAxis + x + 1;

                    // Horizontal
                    if (x != nodesPerAxis - 1)
                        _system.AddSpring(current, right, spacingX, SpringConstant, DampingConstant);

                    // Vertical
                    if (y != nodesPerAxis - 1)
                        _system.AddSpring(current, down, spacingY, SpringConstant, DampingConstant);

                    // Shear
                    if (x != nodesPerAxis - 1 && y != nodesPerAxis - 1)
                    {
                        _system.AddSpring(current, downRight, spacingDiag, SpringConstant, DampingConstant);
                        _system.AddSpring(right, down, spacingDiag, SpringConstant, DampingConstant);
                    }
                }
            }

            // Set indices for drawing
            // Two triangles make each square, so six indices per node except for the last row and column.
            List<uint> indices = new List<uint>(6 * (nodesPerAxis - 1) * (nodesPerAxis - 1));
            for (int y = 0; y < nodesPerAxis - 1; y++)
            {
                for (int x = 0; x < nodesPerAxis - 1; x++)
                {
                    indices.AddRange(new uint[] {
                        (uint)(y*nodesPerAxis + x),
                        (uint)((y+1)*nodesPerAxis + x),
                        (uint)((y+1)*nodesPerAxis + x + 1),
                        (uint)((y*nodesPerAxis) + x),
                        (uint)(y*nodesPerAxis + x + 1),
                        (uint)((y+1)*nodesPerAxis + x + 1)
                    });
                }
            }
            _indicesBuffer.SetData(indices.ToArray());
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            var positions = _system.GetPositions().ToArray();
            _vertexBuffer.SetData(positions);
            _normalsBuffer.SetData(positions);
        }

        public void Update(float deltaTime)
        {
            _system.SetExternalAcceleration((node, dt) => -Gravity*Vector3.UnitY + _pushAcceleration(node));
            _system.Step(deltaTime, MinimumTimeStep);
            UpdatePositions();
            _pushAcceleration = n => Vector3.Zero;
        }

        /// <summary>
        /// Imagine a sausage shape starting at "startSausage" and ending at "endSausage" with radius "radius".
        /// Push any points within the sausage by "pushAmount".
        /// </summary>
        public void Push(Vector3 startSausage, Vector3 endSausage, float radius, float pushAmount, float deltaTime)
        {
            _pushAcceleration = PushMath.PushAccelerationFunction(startSausage, endSausage, radius, pushAmount, deltaTime);
        }
    }
}
