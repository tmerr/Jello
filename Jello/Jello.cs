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
            CursorVisible = false;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var delta = CalculateDelta();
            if (delta.X != 0 || delta.Y != 0)
            {
                _camera.Turn(delta.X/500f, delta.Y/500f);
            }

            var kState = OpenTK.Input.Keyboard.GetState();
            if (kState.IsKeyDown(Key.W))
                _camera.Forward(5);
            if (kState.IsKeyDown(Key.S))
                _camera.Forward(-5);
            if (kState.IsKeyDown(Key.A))
                _camera.Strafe(-2);
            if (kState.IsKeyDown(Key.D))
                _camera.Strafe(2);

            OpenTK.Input.Mouse.SetPosition(WindowCenter.X, WindowCenter.Y);
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

        MouseState previous;

        Point CalculateDelta()
        {
            var current = OpenTK.Input.Mouse.GetState();
            Point delta = new Point(0, 0);
            if (current != previous)
            {
                // Mouse state has changed
                delta = new Point(current.X - previous.X, current.Y - previous.Y); 
                int xdelta = current.X - previous.X;
                int ydelta = current.Y - previous.Y;
            }
            previous = current;
            return delta;
        }
    }
}
