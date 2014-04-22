using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using Jello.Rendering;
using Jello.Entities;

namespace Jello
{
    class MainWindow : GameWindow
    {
        private readonly Camera _camera;
        private readonly Renderer _renderer;
        private readonly TestObject test;
        private readonly Cloth cloth;
        private readonly JelloEntity jello;

        public MainWindow()
	    : base(1200, 800)
        {
            _camera = new Camera();
            _renderer = new Renderer();
            _renderer.Use();
            CursorVisible = false;
            test = new TestObject();
            cloth = new Cloth();
            jello = new JelloEntity();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            float deltaTime = 0.012f;

            var delta = CalculateDelta();
            if (delta.X != 0 || delta.Y != 0)
            {
                _camera.Turn(delta.X/800f, delta.Y/800f);
            }

            var kState = OpenTK.Input.Keyboard.GetState();
            if (kState.IsKeyDown(Key.W))
                _camera.Forward(2);
            if (kState.IsKeyDown(Key.S))
                _camera.Forward(-2);
            if (kState.IsKeyDown(Key.A))
                _camera.Strafe(-1);
            if (kState.IsKeyDown(Key.D))
                _camera.Strafe(1);
            if (kState.IsKeyDown(Key.Space))
            {
                jello.Push(_camera.Location, _camera.Location + _camera.FacingVector*10, 10f, 1f, deltaTime);
                cloth.Push(_camera.Location, _camera.Location + _camera.FacingVector*10, 10f, 1f, deltaTime);
            }

            OpenTK.Input.Mouse.SetPosition(WindowCenter.X, WindowCenter.Y);

            cloth.Update(deltaTime);
            jello.Update(deltaTime);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _renderer.Render(_camera, Color.Aquamarine, new List<ModelData>(new ModelData[] { test.ModelData, cloth.ModelData, jello.ModelData }));
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

        private MouseState previous;

        private Point CalculateDelta()
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
