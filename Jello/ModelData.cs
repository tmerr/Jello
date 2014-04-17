using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello
{
    sealed class ModelData
    {
        public VertexBuffer<Vector3> Vertices { get; private set; }

        public VertexBuffer<Vector3> Normals { get; private set; }

        public VertexBuffer<uint> Indices { get; private set; }

        public ModelData(VertexBuffer<Vector3> vertices, VertexBuffer<Vector3> normals, VertexBuffer<uint> indices)
        {
            Vertices = vertices;
            Normals = normals;
            Indices = indices;
        }

        public ModelData(Vector3[] vertices, Vector3[] normals, uint[] indices)
        {
            Vertices = new VertexBuffer<Vector3>();
            Vertices.SetData(vertices);

            Normals = new VertexBuffer<Vector3>();
            Normals.SetData(normals);

            Indices = new VertexBuffer<uint>();
            Indices.SetData(indices);
        }

        public void Dispose()
        {
            Vertices.Dispose();
            Normals.Dispose();
            Indices.Dispose();
        }
    }
}
