using System;
using OpenTK;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelloUnitTests.Physics
{
    [TestClass]
    public class CollisionResolutionTests
    {
        [TestMethod]
        public void EnsureLineGoingThroughTriangleIsDetected()
        {
            Vector3 origin = new Vector3(0, 5, -10);
            Vector3 direction = Vector3.UnitZ;

            Vector3 v1 = new Vector3(-10, 0, 0);
            Vector3 v2 = new Vector3(10, 0, 0);
            Vector3 v3 = new Vector3(0, 10, 0);

            var intersection = Jello.Physics.CollisionResolution.RayTriangleIntersection(origin, direction, v1, v2, v3);

            Assert.IsTrue(intersection.Item1);
        }

        [TestMethod]
        public void EnsureLineNotGoingThroughTriangleIsNotDetected()
        {
            Vector3 origin = new Vector3(0, 5, -10);
            Vector3 direction = Vector3.UnitY;

            Vector3 v1 = new Vector3(-10, 0, 0);
            Vector3 v2 = new Vector3(10, 0, 0);
            Vector3 v3 = new Vector3(0, 10, 0);

            var intersection = Jello.Physics.CollisionResolution.RayTriangleIntersection(origin, direction, v1, v2, v3);

            Assert.IsFalse(intersection.Item1);
        }
    }
}
