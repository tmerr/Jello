using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Physics
{
    static class PushMath
    {
        /// <summary>
        /// Push everything within the sausage shape that starts at "startSausage" and ends at "endSausage" with radius "radius", with a max amount that is
        /// "pushAmount" that falls off the farther away you are.
        /// </summary>
        public static Func<Node, Vector3> PushAccelerationFunction(Vector3 startSausage, Vector3 endSausage, float radius, float pushAmount, float deltaTime)
        {
            return node =>
            {
                float maxDistance = (endSausage - startSausage).Length + radius;
                float distance = PointToSegmentDistance(node.Position, startSausage, endSausage);
                if (distance < radius)
                {
                    var force = (endSausage - startSausage).Normalized() * pushAmount * ((maxDistance- distance) / maxDistance);
                    return force / node.Mass;
                }
                return Vector3.Zero;
            };
        }

        /// <summary>
        /// http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
        /// </summary>
        public static float PointToLineDistance(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
           return Vector3.Cross(lineEnd - lineStart, lineStart - point).Length / (lineEnd - lineStart).Length;
        }

        /// <summary>
        /// http://geomalgorithms.com/a02-_lines.html
        /// </summary>
        public static float PointToSegmentDistance(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            Vector3 v = segmentEnd - segmentStart;
            Vector3 w = point - segmentStart;

            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
                return (segmentStart - point).Length;

            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
                return (segmentEnd - point).Length;

            float b = c1 / c2;
            Vector3 pb = segmentStart + Vector3.Mult(v, b);
            return (pb - point).Length;
        }
    }
}
