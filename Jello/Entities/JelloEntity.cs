using Jello.Physics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Jello.Entities
{
    class JelloEntity
    {
        private VertexBuffer<Vector3> _vertexBuffer;
        private VertexBuffer<Vector3> _normalsBuffer;
        private VertexBuffer<uint> _indicesBuffer;

        private List<uint> _indices;

        MassSpringSystem _system = new MassSpringSystem(IntegratorType.RungeKutta4);

        private const float Gravity = 9.8f;
        private const float SpringConstant = 20f;
        private const float DampingConstant = 0.1f;
        private const float NodeMass = 0.1f; // in kg
        private float MinimumTimeStep = 0.01f;
        private Func<Node, Vector3> _pushAcceleration = i => Vector3.Zero;

        public ModelData ModelData
        {
            get
            {
                return new ModelData(_vertexBuffer, _normalsBuffer, _indicesBuffer);
            }
        }
        public JelloEntity()
        {
            _vertexBuffer = new VertexBuffer<Vector3>();
            _normalsBuffer = new VertexBuffer<Vector3>();
            _indicesBuffer = new VertexBuffer<uint>();

            ArrangeNodes(new Vector3(0, 3, 4), new Vector3(3, 3, 3), new Point3(5, 5, 5));
        }

        /// <summary> Smallest tick is x, next is y, next is z </summary>
        private static int Index3To1(int x, int y, int z, Point3 dimensions)
        {
            return z * dimensions.Y * dimensions.X + y * dimensions.X + x;
        }

        private static List<uint> GetFaceIndices(Func<int, int, int> index3To1Call, Point dimensions)
        {
            var indices = new List<uint>(dimensions.X * dimensions.Y);
            for (int i = 0; i < dimensions.X - 1; i++)
                for (int j = 0; j < dimensions.Y -1; j++)
                {
                    indices.Add((uint)index3To1Call(i, j));
                    indices.Add((uint)index3To1Call(i + 1, j));
                    indices.Add((uint)index3To1Call(i + 1, j + 1));
                    indices.Add((uint)index3To1Call(i, j));
                    indices.Add((uint)index3To1Call(i, j + 1));
                    indices.Add((uint)index3To1Call(i + 1, j + 1));
                }
            return indices;
        }

        private void ArrangeNodes(Vector3 bottomLeft, Vector3 size, Point3 nodesPerAxis)
        {
            // The cube region that can fit between each node
            Vector3 spacing = new Vector3(size.X/nodesPerAxis.X, size.Y/nodesPerAxis.Y, size.Z/nodesPerAxis.Z);

            for (int z = 0; z < nodesPerAxis.Z; z++)
            {
                for (int y = 0; y < nodesPerAxis.Y; y++)
                {
                    for (int x = 0; x < nodesPerAxis.X; x++)
                    {
                        _system.AddNode(bottomLeft + new Vector3(x * spacing.X, y * spacing.Y, z * spacing.Z), NodeMass);
                        int current = Index3To1(x, y, z, nodesPerAxis);

                        // Structural
                        if (x != nodesPerAxis.X-1)
                            _system.AddSpring(current, Index3To1(x+1, y, z, nodesPerAxis), spacing.X, SpringConstant, DampingConstant);
                        if (y != nodesPerAxis.Y - 1)
                            _system.AddSpring(current, Index3To1(x, y+1, z, nodesPerAxis), spacing.Y, SpringConstant, DampingConstant);
                        if (z != nodesPerAxis.Z - 1)
                            _system.AddSpring(current, Index3To1(x, y, z + 1, nodesPerAxis), spacing.Z, SpringConstant, DampingConstant);
 

                        // Shear
                        // xy
                        if (x != nodesPerAxis.X - 1 && y != nodesPerAxis.Y - 1)
                        {
                            _system.AddSpring(current, Index3To1(x + 1, y + 1, z, nodesPerAxis), spacing.Xy.Length, SpringConstant, DampingConstant);
                            _system.AddSpring(Index3To1(x, y + 1, z, nodesPerAxis), Index3To1(x + 1, y, z, nodesPerAxis), spacing.Xy.Length, SpringConstant, DampingConstant);
                        }
                        // yz
                        if (y != nodesPerAxis.Y - 1 && z != nodesPerAxis.Z - 1)
                        {
                            _system.AddSpring(current, Index3To1(x, y + 1, z + 1, nodesPerAxis), spacing.Yz.Length, SpringConstant, DampingConstant);
                            _system.AddSpring(Index3To1(x, y + 1, z, nodesPerAxis), Index3To1(x, y, z + 1, nodesPerAxis), spacing.Yz.Length, SpringConstant, DampingConstant);
                        }
                        // xz
                        if (x != nodesPerAxis.X - 1 && z != nodesPerAxis.Z - 1)
                        {
                            _system.AddSpring(current, Index3To1(x + 1, y, z + 1, nodesPerAxis), spacing.Xz.Length, SpringConstant, DampingConstant);
                            _system.AddSpring(Index3To1(x + 1, y, z, nodesPerAxis), Index3To1(x, y, z+1, nodesPerAxis), spacing.Xz.Length, SpringConstant, DampingConstant);
                        }

                        // Bend
                        if (x < nodesPerAxis.X - 2)
                            _system.AddSpring(current, Index3To1(x + 2, y, z, nodesPerAxis), spacing.X * 2, SpringConstant, DampingConstant);
                        if (y < nodesPerAxis.Y - 2)
                            _system.AddSpring(current, Index3To1(x, y + 2, z, nodesPerAxis), spacing.Y * 2, SpringConstant, DampingConstant);
                        if (z < nodesPerAxis.Z - 2)
                            _system.AddSpring(current, Index3To1(x, y, z + 2, nodesPerAxis), spacing.Z * 2, SpringConstant, DampingConstant);
                    }
                }
            }

            _system.AddAnchor(new StaticAnchor(new Vector3(-10, 0, 10)));
            _system.AddAnchor(new StaticAnchor(new Vector3(-10, 0, -10)));
            _system.AddAnchor(new StaticAnchor(new Vector3(10, -4, -10)));
            _system.AddAnchor(new StaticAnchor(new Vector3(10, -4, 10)));

            _indices = new List<uint>();
            var front = GetFaceIndices((i, j) => Index3To1(i, j, 0, nodesPerAxis), new Point(nodesPerAxis.X, nodesPerAxis.Y));
            var back = GetFaceIndices((i, j) => Index3To1(i, j, nodesPerAxis.Z - 1, nodesPerAxis), new Point(nodesPerAxis.X, nodesPerAxis.Y));
            var left = GetFaceIndices((i, j) => Index3To1(0, i, j, nodesPerAxis), new Point(nodesPerAxis.Y, nodesPerAxis.Z));
            var right = GetFaceIndices((i, j) => Index3To1(nodesPerAxis.X - 1, i, j, nodesPerAxis), new Point(nodesPerAxis.Y, nodesPerAxis.Z));
            var bottom = GetFaceIndices((i, j) => Index3To1(i, 0, j, nodesPerAxis), new Point(nodesPerAxis.X, nodesPerAxis.Z));
            var top = GetFaceIndices((i, j) => Index3To1(i, nodesPerAxis.Y - 1, j, nodesPerAxis), new Point(nodesPerAxis.X, nodesPerAxis.Z));
            foreach (var face in new[] { front, back, left, right, bottom, top })
                _indices.AddRange(face);

            _indices.Add((uint)(nodesPerAxis.X * nodesPerAxis.Y * nodesPerAxis.Z + 0));
            _indices.Add((uint)(nodesPerAxis.X * nodesPerAxis.Y * nodesPerAxis.Z + 1));
            _indices.Add((uint)(nodesPerAxis.X * nodesPerAxis.Y * nodesPerAxis.Z + 2));
            _indices.Add((uint)(nodesPerAxis.X * nodesPerAxis.Y * nodesPerAxis.Z + 0));
            _indices.Add((uint)(nodesPerAxis.X * nodesPerAxis.Y * nodesPerAxis.Z + 2));
            _indices.Add((uint)(nodesPerAxis.X * nodesPerAxis.Y * nodesPerAxis.Z + 3));

            _indicesBuffer.SetData(_indices.ToArray());
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
            _system.SetExternalAcceleration((node, dt) => _pushAcceleration(node) -Gravity * Vector3.UnitY);
            _system.Step(deltaTime, MinimumTimeStep, _indices);
            UpdatePositions();
            _pushAcceleration = n => Vector3.Zero;
        }

        /// <summary>
        /// Imagine a sausage shape starting at "startSausage" and ending at "endSausage" with radius "radius".
        /// Push any points within the sausage by "pushAmount".
        /// </summary>
        public void Push(Vector3 startSausage, Vector3 endSausage, float radius, float pushAmount, float deltaTime)
        {
            _pushAcceleration =
                Node =>
                {
                    return PushMath.CalculatePushForce(Node.Position, startSausage, endSausage, radius, pushAmount, deltaTime) / Node.Mass;
                };
        }
    }
}
