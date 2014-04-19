using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jello.Entities.Physics;

namespace Jello.Entities
{
    class Cloth
    {
        private VertexBuffer<Vector3> _vertexBuffer;
        private VertexBuffer<Vector3> _normalsBuffer;
        private VertexBuffer<uint> _indicesBuffer;

        private List<INode> _nodes = new List<INode>();
        private List<Spring> _springs = new List<Spring>();

        private const float Gravity = 9.8f;
        private const float SpringConstant = 100f;
        private const float DampeningConstant = 1.5f;
        private const float NodeMass = 0.1f;

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

            // Create nodes
            for (int y = 0; y < nodesPerAxis - 1; y++)
            {
                for (int x = 0; x < nodesPerAxis; x++)
                {
                    _nodes.Add(new Node(bottomLeft + new Vector3(x * spacingX, y * spacingY, 0f), NodeMass));
                }
            }
            for (int x = 0; x < nodesPerAxis; x++)
            {
                _nodes.Add(new AnchorNode(bottomLeft + new Vector3(x * spacingX, (nodesPerAxis-1) * spacingY, 0f)));
            }

            // Create springs
            for (int y = 0; y < nodesPerAxis; y++)
            {
                for (int x = 0; x < nodesPerAxis; x++)
                {
                    var current = _nodes[y*nodesPerAxis + x];

                    // Horizontal
                    if (x != nodesPerAxis - 1)
                    {
                        var right = _nodes[y * nodesPerAxis + x + 1];
                        _springs.Add(new Spring(current, right, spacingX, SpringConstant, DampeningConstant));
                    }

                    // Vertical
                    if (y != nodesPerAxis - 1)
                    {
                        var down = _nodes[(y + 1) * nodesPerAxis + x];
                        _springs.Add(new Spring(current, down, spacingX, SpringConstant, DampeningConstant));
                    }

                    // Shear
                    if (x != nodesPerAxis - 1 && y != nodesPerAxis - 1)
                    {
                        var down = _nodes[(y + 1) * nodesPerAxis + x];
                        var right = _nodes[y * nodesPerAxis + x + 1];
                        var downRight = _nodes[(y + 1) * nodesPerAxis + x + 1];
                        float length = new Vector2(spacingX, spacingY).Length;
                        _springs.Add(new Spring(current, downRight, length, SpringConstant, DampeningConstant));
                        _springs.Add(new Spring(right, down, length, SpringConstant, DampeningConstant));
                    }
                }
            }

            // Set indices for drawing
            // Two triangles make each square, so six indices per node except for the last row and column.
            List<uint> indices = new List<uint>(6*(nodesPerAxis-1)*(nodesPerAxis-1));
            for (int y = 0; y < nodesPerAxis-1; y++)
            {
                for (int x = 0; x < nodesPerAxis-1; x++)
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
            UpdateBuffers();
        }

        private void UpdateBuffers()
        {
            var positions = _nodes.Select(n => n.Position).ToArray();
            _vertexBuffer.SetData(positions);
            _normalsBuffer.SetData(positions);
        }

        public void Update(float deltaTime)
        {
            foreach (var node in _nodes)
            {
                node.ApplyAcceleration(new Vector3(0, -Gravity, 0), deltaTime);
            }
            
            foreach (var spring in _springs)
            {
                spring.ApplyForceToNodes(deltaTime);
            }

            foreach (var node in _nodes)
            {
                node.CrunchVelocities(deltaTime);
            }

            UpdateBuffers();
        }

        /// <summary>
        /// Imagine the toward vector as a line extending from source. Every point within "radius" of that line forms a sausage shape. If any
        /// points are within said sausage, push them in the direction of toward by amount pushAmount.
        /// </summary>
        public void Push(Vector3 startSausage, Vector3 endSausage, float radius, float pushAmount, float deltaTime)
        {
            foreach(var node in _nodes)
            {
                float distance = PointToLineDistance(node.Position, startSausage, endSausage);
                if (distance < radius)
                {
                    node.ApplyForce((endSausage - startSausage).Normalized() * pushAmount, deltaTime);
                }
            }
        }

        public static float PointToLineDistance(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 v = lineEnd - lineStart;
            Vector3 w = point - lineStart;
            float c1 = Vector3.Dot(w, v);
            float c2 = Vector3.Dot(v, v);
            float b = c1 / c2;
            Vector3 between = lineStart + b * v;
            return between.Length;
        }
    }
}
