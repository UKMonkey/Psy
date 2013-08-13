using SlimMath;

namespace Psy.Core.Collision
{
    public static class BoundingSphereExtensions
    {
        /// <summary>
        /// Determines whether there is an intersection between a <see cref="SlimMath.BoundingSphere"/> and a triangle.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triagnle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <param name="point">Point of collision</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool Intersects(this BoundingSphere sphere, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out Vector3 point)
        {
            //Source-Source: SlimDX

            //Source: Real-Time Collision Detection by Christer Ericson
            //Reference: Page 167

            SlimMath.Collision.ClosestPointOnTriangleToPoint(ref vertex1, ref vertex2, ref vertex3, ref sphere.Center, out point);
            var v = point - sphere.Center;

            float dot;
            Vector3.Dot(ref v, ref v, out dot);

            return dot <= sphere.Radius * sphere.Radius;
        }

        /// <summary>
        /// Determines whether there is an intersection between a <see cref="SlimMath.BoundingBox"/> and a <see cref="SlimMath.BoundingSphere"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="vector">Intersect point</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool Intersects(this BoundingSphere sphere, ref BoundingBox box, out Vector3 vector)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //Reference: Page 166

            Vector3.Clamp(ref sphere.Center, ref box.Minimum, ref box.Maximum, out vector);
            float distance = Vector3.DistanceSquared(sphere.Center, vector);

            return distance <= sphere.Radius * sphere.Radius;
        }

        public static bool Intersects(this BoundingSphere sphere, ref BoundingSphere other, out float distance)
        {
            var radiisum = sphere.Radius + other.Radius;
            distance = Vector3.Distance(sphere.Center, other.Center);
            return distance <= radiisum;
        }
    }

}