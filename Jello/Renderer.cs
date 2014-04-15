using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Jello
{
    class Renderer
    {
        public Renderer()
        {
        }

        public void Initialize()
        {
        }

        public void Render(Camera camera)
        {
        }

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
            GL.Vertex3(-10f, 0f, 10f);
            GL.Color3(0, 0, 255);
            GL.Vertex3(10f, 0f, 10f);
            GL.Color3(0, 0, 255);
            GL.Vertex3(0f, 15f, 10f);
            GL.End();
        }
    }
}