using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Jello
{
    class Jello : GameWindow
    {
        private readonly Camera _camera;
        private readonly Renderer _renderer;

        public Jello()
	    : base(1200, 800)
        {
            _camera = new Camera();
            _renderer = new Renderer();
            GL.Viewport(0, 0, this.Width, this.Height);
            Mouse.Move += (a, b) => _camera.Turn(b.XDelta/200f, b.YDelta/200f);
            //CursorVisible = false;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var kState = OpenTK.Input.Keyboard.GetState();
            if (kState.IsKeyDown(Key.W))
                _camera.Forward(5);
            if (kState.IsKeyDown(Key.S))
                _camera.Forward(-5);
            if (kState.IsKeyDown(Key.A))
                _camera.Strafe(-2);
            if (kState.IsKeyDown(Key.D))
                _camera.Strafe(2);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _renderer.RenderTestTriangle(_camera);
            this.SwapBuffers();
        }

        private Point WindowCenter
        {
            get
            {
                return new Point(
                    (Bounds.Left + Bounds.Right) / 2,
                    (Bounds.Top + Bounds.Bottom) / 2);
            }
        }
    }
}
