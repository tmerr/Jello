using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello
{
    class StandardBufferGroup
    {
        public VertexBuffer<Vector3> Vertices { get; private set; }

        public VertexBuffer<Vector3> Normals { get; private set; }

        public VertexBuffer<uint> Indices { get; private set; }

        public StandardBufferGroup(VertexBuffer<Vector3> vertices, VertexBuffer<Vector3> normals, VertexBuffer<uint> indices)
        {
            Vertices = vertices;
            Normals = normals;
            Indices = indices;
        }
    }
}
