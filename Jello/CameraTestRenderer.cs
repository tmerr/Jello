using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello
{
    class CameraTestRenderer
    {
        public void RenderTestTriangle(Camera camera)
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            var projectionMatrix = camera.GetProjectionMatrix();
            GL.LoadMatrix(ref projectionMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(0, 0, 255);
            GL.Vertex3(-10f, 0f, 50f);
            GL.Color3(0, 0, 255);
            GL.Vertex3(10f, 0f, 50f);
            GL.Color3(0, 0, 255);
            GL.Vertex3(0f, 15f, 50f);
            GL.End();
        }
    }
}
