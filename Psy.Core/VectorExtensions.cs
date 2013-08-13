using System;
using SlimMath;

namespace Psy.Core
{
    public static class VectorExtensions
    {
        public static bool IsZeroLength(this Vector2 me)
        {
            return Math.Abs(me.LengthSquared) < 0.001f;
        }

        public static bool IsZeroLength(this Vector3 me)
        {
            return Math.Abs(me.LengthSquared) < 0.001f;
        }

        public static Vector3 DivideBy(this Vector3 me, Vector3 other)
        {
            var x = me.X/other.X;
            var y = me.Y/other.Y;
            var z = me.Z/other.Z;

            if (float.IsNaN(x) || float.IsInfinity(x)) { x = 0.0f; }
            if (float.IsNaN(y) || float.IsInfinity(y)) { y = 0.0f; }
            if (float.IsNaN(z) || float.IsInfinity(z)) { z = 0.0f; }

            return new Vector3(x, y, z);
        }

        public static Vector3 Translate(this Vector3 me, float x, float y, float z)
        {
            return me + new Vector3(x, y, z);
        }

        public static Vector2 Translate(this Vector2 me, float x, float y)
        {
            return me + new Vector2(x, y);
        }

        public static Vector3 Scale(this Vector3 me, Vector3 other)
        {
            return Vector3.Modulate(me, other);
        }

        public static Vector2 AsVector2(this Vector3 me)
        {
            return new Vector2(me.X, me.Y);
        }

        public static Vector3 AsVector3(this Vector2 me)
        {
            return new Vector3(me.X, me.Y, 0);
        }

        public static Vector4 AsVector4(this Vector3 me)
        {
            return new Vector4(me.X, me.Y, me.Z, 1);
        }

        public static Vector3 AsVector3(this Vector4 me)
        {
            return new Vector3(me.X, me.Y, me.Z);
        }

        public static Vector4 AsVector4(this Vector2 me)
        {
            return new Vector4(me.X, me.Y, 0, 1);
        }

        public static Vector3 NormalizeRet(this Vector3 me)
        {
            return Vector3.Normalize(me);
        }

        public static Vector2 NormalizeRet(this Vector2 me)
        {
            return Vector2.Normalize(me);
        }

        public static Vector3 Cross(this Vector3 me, Vector3 other)
        {
            return Vector3.Cross(me, other);
        }

        public static float Dot(this Vector3 me, Vector3 other)
        {
            return Vector3.Dot(me, other);
        }

        public static float Distance(this Vector2 me, Vector2 other)
        {
            return Vector2.Distance(me, other);
        }

        public static float Distance(this Vector3 me, Vector3 other)
        {
            return Vector3.Distance(me, other);
        }

        public static float DistanceSquared(this Vector2 me, Vector2 other)
        {
            return Vector2.DistanceSquared(me, other);
        }

        public static float DistanceSquared(this Vector3 me, Vector3 other)
        {
            return Vector3.DistanceSquared(me, other);
        }

        public static Vector3 From2DAngle(float angle, float magnitude = 1.0f)
        {
            var x = (float)Math.Cos(angle) * magnitude;
            var y = (float)Math.Sin(angle) * magnitude;
            return new Vector3(x, y, 0);
        }

        public static float AngleTo2DPoint(this Vector2 me, Vector2 other)
        {
            var ofInterest = other - me;
            return ofInterest.ZPlaneAngle();
        }

        public static float AngleTo2DPoint(this Vector3 me, Vector3 other)
        {
            var ofInterest = other - me;
            return ofInterest.ZPlaneAngle();
        }

        public static float ZPlaneAngle(this Vector2 me)
        {
            return (float) Math.Atan2(me.Y, me.X);
        }

        public static float ZPlaneAngle(this Vector3 me)
        {
            return (float)Math.Atan2(me.Y, me.X);
        }

        public static float FastDistance(this Vector3 me, Vector3 other)
        {
            var xdist = Math.Abs(other.X - me.X);
            var ydist = Math.Abs(other.Y - me.Y);
            var zdist = Math.Abs(other.Z - me.Z);
            return xdist + ydist + zdist;
        }

        /// <summary>
        /// Performs a rotation on the z plane (ie z remains the same)
        /// </summary>
        /// <param name="me"> </param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 me, float angle)
        {
            var x = (float)(me.X * Math.Cos(angle) - me.Y * Math.Sin(angle));
            var y = (float)(me.X * Math.Sin(angle) + me.Y * Math.Cos(angle));

            return new Vector2(x, y);
        }

        /// <summary>
        /// Performs a rotation on the z plane (ie z remains the same)
        /// </summary>
        /// <param name="me"> </param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 Rotate(this Vector3 me, float angle)
        {
            var x = (float)(me.X * Math.Cos(angle) - me.Y * Math.Sin(angle));
            var y = (float)(me.X * Math.Sin(angle) + me.Y * Math.Cos(angle));

            return new Vector3(x, y, me.Z);
        }

        public static Vector2 Scale(this Vector2 me, float xScale, float yScale)
        {
            return new Vector2(me.X * xScale, me.Y * yScale);
        }

        public static Vector2 InvScale(this Vector2 me, float xScale, float yScale)
        {
            return new Vector2(me.X / xScale, me.Y / yScale);
        }

        public static Vector2 InvScale(this Vector2 me, Vector2 other)
        {
            return new Vector2(me.X / other.X, me.Y / other.Y);
        }

        public static Vector3 Scale(this Vector3 me, float xScale, float yScale, float zScale)
        {
            return new Vector3(me.X * xScale, me.Y * yScale, me.Z * yScale);
        }

        public static Vector3 InvScale(this Vector3 me, float xScale, float yScale, float zScale)
        {
            return new Vector3(me.X / xScale, me.Y / yScale, me.Z * yScale);
        }

        public static Vector2 GetClosestEnd(Vector2 start, Vector2 a, Vector2 b, Vector2 direction, out bool invalidEnd)
        {
            // it is assumed that a & b are joined by direction, but it's not known if 
            // it's a to b, or b to a... this returns a in the former, and b in the latter

            invalidEnd = true;

            if ((a.X - start.X) * direction.X < 0 ||
                (a.Y - start.Y) * direction.Y < 0)
                return b;

            if ((b.X - start.X) * direction.X < 0 ||
                (b.Y - start.Y) * direction.Y < 0)
                return a;

            invalidEnd = false;

            if (direction.X > 0)
                return a.X < b.X ? a : b;

            if (direction.X < 0)
                return a.X > b.X ? a : b;

            if (direction.Y > 0)
                return a.Y < b.Y ? a : b;

            return a.Y > b.Y ? a : b;
        }

        public static Vector3 GetClosestEnd(Vector3 start, Vector3 a, Vector3 b, Vector3 direction, out bool invalidEnd)
        {
            // it is assumed that a & b are joined by direction, but it's not known if 
            // it's a to b, or b to a... this returns a in the former, and b in the latter

            invalidEnd = true;

            if ((a.X - start.X) * direction.X < 0 ||
                (a.Y - start.Y) * direction.Y < 0)
                return b;

            if ((b.X - start.X) * direction.X < 0 ||
                (b.Y - start.Y) * direction.Y < 0)
                return a;

            invalidEnd = false;

            if (direction.X > 0)
                return a.X < b.X ? a : b;

            if (direction.X < 0)
                return a.X > b.X ? a : b;

            if (direction.Y > 0)
                return a.Y < b.Y ? a : b;

            return a.Y > b.Y ? a : b;
        }
    }
}