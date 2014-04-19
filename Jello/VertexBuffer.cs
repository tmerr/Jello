using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using System.Runtime.InteropServices;

namespace Jello
{
    sealed class VertexBuffer<T>
	where T : struct
    {
        int id;

        public int Id
        {
            get
            {
                // Create an id on first use.
                if (id == 0)
                {
                    GraphicsContext.Assert();

                    GL.GenBuffers(1, out id);
                    if (id == 0)
                        throw new Exception("Could not create VBO.");
                }

                return id;
            }
        }

        public int ElementSize { get; private set; }

        public int ElementCount { get; private set; }

        public VertexBuffer()
        {
            ElementCount = 0;
            ElementSize = 0;
        }

        public void SetData(T[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var stride = Marshal.SizeOf(default(T));

            GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Length * stride), data, BufferUsageHint.StaticDraw);

            ElementCount = data.Length;
            ElementSize = stride;
        }

        public void Dispose()
        {
            if (id != -1)
                GL.DeleteBuffer(id);
        }
    }
}
