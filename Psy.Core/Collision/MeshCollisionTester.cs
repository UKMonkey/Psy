using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlimMath;

namespace Psy.Core.Collision
{
    public class MeshCollisionResult
    {
        public CollisionResult RayCollisionResult { get; private set; }
        public byte TriangleId { get; private set; }
        public MeshTriangle Triangle { get; private set; }

        public MeshCollisionResult(CollisionResult rayCollisionResult, byte triangleId = 0, MeshTriangle hit = null)
        {
            RayCollisionResult = rayCollisionResult;
            TriangleId = triangleId;
            Triangle = hit;
        }
    }


    class MeshCollisionData
    {
        private static readonly MeshCollisionResult UncollidedCollisionResult =
                new MeshCollisionResult(new CollisionResult {HasCollided = false});

        public bool HasCollided { get; set; }
        public byte CollisionId { get; set; }
        public MeshTriangle Triangle { get; set; }
        public CollisionResult CollisionResult { get; set; }
        public float CollisionDistance { get; set; }

        public MeshCollisionData()
        {
            HasCollided = false;
        }

        public MeshCollisionResult GetCollisionResult()
        {
            return HasCollided ? new MeshCollisionResult(CollisionResult, CollisionId, Triangle) : UncollidedCollisionResult;
        }
    }

    public class MeshCollisionTester : IMeshCollisionTester
    {
        private readonly Mesh _mesh;

        public MeshCollisionTester(Mesh mesh)
        {
            _mesh = mesh;
            if (_mesh == null)
            {
                throw new Exception("Mesh cannot be null");
            }
        }

        public IEnumerable<Mesh> Meshes
        {
            get { return new List<Mesh> { _mesh }; }
        }

        private void UpdateCollisionData(MeshCollisionData data, float distance, CollisionResult result, MeshTriangle triangle)
        {
            if (!data.HasCollided ||
                distance < data.CollisionDistance)
            {
                data.CollisionDistance = distance;
                data.CollisionResult = result;
                data.CollisionId = triangle.Id;
                data.Triangle = triangle;
                data.HasCollided = true;
            }
        }

        private MeshCollisionResult CollideInSeries(IEnumerable<MeshTriangle> triangles, Vector3 point, Vector3 direction)
        {
            var data = new MeshCollisionData();

            foreach (var triangle in triangles)
            {
                var result = CollideTriangleWithRay(triangle, point, direction);
                if (!result.HasCollided)
                    continue;

                var realTriangle = triangle.Translate(_mesh.Translation);
                var distance = result.CollisionPoint.Distance(point);
                result.CollisionPoint += _mesh.Translation;
                result.CollisionMesh = _mesh;

                UpdateCollisionData(data, distance, result, realTriangle);
            }

            return data.GetCollisionResult();
        }

        private MeshCollisionResult CollideInParallel(IEnumerable<MeshTriangle> triangles, Vector3 point, Vector3 direction)
        {
            var data = new MeshCollisionData();
            var lockingObject = new Mutex();

            Parallel.ForEach(triangles, triangle =>
            {
                var result = CollideTriangleWithRay(triangle, point, direction);
                if (!result.HasCollided)
                    return;

                var distance = result.CollisionPoint.Distance(point);
                var realTriangle = triangle.Translate(_mesh.Translation);
                result.CollisionPoint += _mesh.Translation;
                lock (lockingObject)
                {
                    UpdateCollisionData(data, distance, result, realTriangle);
                }
            });

            return data.GetCollisionResult();
        }

        public MeshCollisionResult CollideWithLineSegment(Vector3 point, Vector3 direction, float distanceSquared)
        {
            return CollideWithLineSegment(_mesh, point, direction, distanceSquared);
        }

        public virtual MeshCollisionResult CollideWithRay(Vector3 point, Vector3 direction)
        {
            return CollideWithRay(_mesh, point, direction);
        }

        private MeshCollisionResult CollideWithLineSegment(Mesh target, Vector3 point, Vector3 direction, float distanceSquared)
        {
            var result = CollideWithRay(target, point, direction);

            if (result.RayCollisionResult.HasCollided)
            {
                var colDistanceSqr = result.RayCollisionResult.CollisionPoint.DistanceSquared(point);

                if (colDistanceSqr < distanceSquared)
                    return result;
            }
            return new MeshCollisionResult(new CollisionResult { HasCollided = false }, 0);
        }

        private MeshCollisionResult CollideWithRay(Mesh target, Vector3 point, Vector3 direction)
        {
            var translatedPoint = point - target.Translation;
            int triangleCount;

            var triangles = target.GetAllTriangles(out triangleCount);

            if (triangleCount > 50)
                return CollideInParallel(triangles, translatedPoint, direction);
            return CollideInSeries(triangles, translatedPoint, direction);
        }

        private CollisionResult CollideTriangleWithRay(MeshTriangle triangle, Vector3 point, Vector3 direction)
        {
            var n = triangle.Normal;
            var b = n.Dot(direction);

            if (b >= 0.00000)
                return new CollisionResult { HasCollided = false };

            var w0 = point - triangle.P0;
            var a = -n.Dot(w0);
            var r = a / b;
            if (r < 0)
                return new CollisionResult { HasCollided = false };

            var I = point + r * direction;
            var w = I - triangle.P0;
            var wu = w.Dot(triangle.U);
            var wv = w.Dot(triangle.V);
                
            var s = (triangle.UV * wv - triangle.VV * wu) / triangle.D;
            if (s < 0 || s > 1)
                return new CollisionResult { HasCollided = false };

            var t = (triangle.UV * wu - triangle.UU * wv) / triangle.D;
            if (t < 0 || (s + t) > 1)
                return new CollisionResult { HasCollided = false };

            return new CollisionResult { HasCollided = true, CollisionPoint = I };
        }
    }
}