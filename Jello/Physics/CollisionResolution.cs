using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jello.Physics
{
    class CollisionResolution
    {
        public static List<Node> ResolveCollisions(IReadOnlyList<Node> lastNodes, IReadOnlyList<Node> currentNodes,
                                                                      IReadOnlyList<uint> triangleFaceIndices,
                                                                      float dt)
        {
            var newNodes = currentNodes.ToList();

            for (int n = 0; n < currentNodes.Count; n++)
            {
                for (int i = 0; i < triangleFaceIndices.Count; i += 3)
                {
                    if (!(n == (int)triangleFaceIndices[i] || n == (int)triangleFaceIndices[i + 1] || n == (int)triangleFaceIndices[i + 2]))
                    {
                        var rayOrigin = lastNodes[n].Position;
                        var rayDirection = (currentNodes[n].Position - lastNodes[n].Position).Normalized();
                        var v1 = currentNodes[(int)triangleFaceIndices[i]].Position;
                        var v2 = currentNodes[(int)triangleFaceIndices[i + 1]].Position;
                        var v3 = currentNodes[(int)triangleFaceIndices[i + 2]].Position;
                        Tuple<bool, float> result = RayTriangleIntersection(rayOrigin, rayDirection, v1, v2, v3);
                        bool intersected = result.Item1;
                        float length = result.Item2;

                        if (intersected)
                        {
                            Vector3 intersectionPosition = lastNodes[n].Position + rayDirection * length;
                            Vector3 deltaPosition = currentNodes[n].Position - lastNodes[n].Position;
                            bool inCorrectDirection = Vector3.Dot(rayDirection, deltaPosition) > 0;
                            float maximumLength = (currentNodes[n].Position - lastNodes[n].Position).Length;

                            // Account for error by registering collisions even when slightly out of range
                            maximumLength += 0.001f;
                            bool withinLength = length <= maximumLength;

                            if (inCorrectDirection && withinLength)
                            {
                                var newPosition = intersectionPosition - rayDirection * 0.001f;
                                newNodes[n] = new Node(currentNodes[n].Anchor, currentNodes[n].Mass, newPosition, Vector3.Zero);
                            }

                            /*
                            var normalToTriangle = Vector3.Cross(v3 - v1, v2 - v1).Normalized();
                            var penetration = Vector3.Dot(rayDirection, normalToTriangle);
                            var correctlyFacingNormal = - Math.Sign(penetration) * normalToTriangle;
                        
                            var normalForce = something something correctlyFacingNormal * Math.Abs(penetration);
                            */
                        }
                    }
                }
            }

            return newNodes;
        }

        private const float Epsilon = 0.00001f;
        /// <summary>
        /// Use the Moller-Trumbore intersection algorithm to check whether a ray intersects a triangle.
        /// A ray is a line that starts at ro and extends infinitely in the direction of rd.
        /// </summary>
        /// <param name="ro">Ray origin</param>
        /// <param name="rd">Ray direction</param>
        /// <param name="v2">Triangle point 1</param>
        /// <param name="t2">Triangle point 2</param>
        /// <param name="t3">Triangle point 3</param>
        /// <returns>Whether there was an intersection, and how far along it was on the ray</returns>
        public static Tuple<bool, float> RayTriangleIntersection(Vector3 ro, Vector3 rd, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var edge1 = v2 - v1;
            var edge2 = v3 - v1;

            var P = Vector3.Cross(rd, edge2);
            float determinant = Vector3.Dot(edge1, P);

            if (Math.Abs(determinant) < Epsilon)
                return Tuple.Create(false, -1f);
            float inverseDeterminant = 1f / determinant;

            var T = ro - v1;

            var u = Vector3.Dot(T, P) * inverseDeterminant;
            if (u < 0 || u > 1)
                return Tuple.Create(false, -1f);

            Vector3 Q = Vector3.Cross(T, edge1);
            float v = Vector3.Dot(rd, Q) * inverseDeterminant;

            if (v < 0 || u + v > 1)
                return Tuple.Create(false, -1f);

            float t = Vector3.Dot(edge2, Q) * inverseDeterminant;

            if (t > Epsilon)
                return Tuple.Create(true, t);

            return Tuple.Create(false, -1f);
        }
    }
}
