using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello
{
    class TestObject
    {
        private Vector3[] positionVboData = new Vector3[]{
            new Vector3(-1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3( 1.0f, -1.0f, -1.0f), 
            new Vector3( 1.0f,  1.0f, -1.0f),
            new Vector3(-1.0f,  1.0f, -1.0f) };

        private uint[] indicesVboData = new uint[]{
            // front face
            0, 1, 2, 2, 3, 0,
            // top face
            3, 2, 6, 6, 7, 3,
            // back face
            7, 6, 5, 5, 4, 7,
            // left face
            4, 0, 3, 3, 7, 4,
            // bottom face
            0, 1, 5, 5, 4, 0,
            // right face
            1, 5, 6, 6, 2, 1, };
        
        public ModelData ModelData { get; private set; }

        public TestObject()
        {
            ModelData = new ModelData(positionVboData, positionVboData, indicesVboData);
        }
    }
}
