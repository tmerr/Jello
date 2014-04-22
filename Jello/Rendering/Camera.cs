using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Rendering
{
    class Camera
    {
        private float leftRightAngle;
        private float upDownAngle;
        public Vector3 Location { get; private set; }

        public Camera()
        {
            leftRightAngle = 0;
            upDownAngle = 0;
            Location = Vector3.Zero;
        }

        public Vector3 FacingVector
        {
            get
            {
                return Vector3.Transform(Vector3.UnitZ, Matrix4.CreateRotationX(upDownAngle) * Matrix4.CreateRotationY(leftRightAngle));
            }
        }

        /// <summary>
        /// Turn left/right proportional to X, and up/down proportional to Y.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void Turn(float X, float Y)
        {
            leftRightAngle = Mod(leftRightAngle - X, 2 * (float)Math.PI);
            upDownAngle = Mod(upDownAngle + Y, 2 * (float)Math.PI);
        }

        public void Strafe(float magnitude)
        {
            Location += magnitude * Vector3.Cross(FacingVector, Vector3.UnitY);
        }

        public void Forward(float magnitude)
        {
            Location += magnitude * FacingVector;
        }

        public Matrix4 GetProjectionMatrix()
        {
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1, 1.0f, 1000.0f);
            return Matrix4.LookAt(Location, Location + FacingVector, Vector3.UnitY) * projection;
        }

        private static float Mod(float a, float n)
        {
            return a - (float)Math.Floor(a / n) * n;
        }
    }
}
