using System;
using System.Collections.Generic;
using System.Linq;
using SlimMath;

namespace Psy.Core.Collision
{
    public class MultiMeshCollisionTester : List<IMeshCollisionTester>, IMeshCollisionTester
    {
        public MultiMeshCollisionTester()
        {}

        public MultiMeshCollisionTester(IEnumerable<Mesh> meshes)
            :base(meshes.Select(item => new MeshCollisionTester(item)))
        {}

        public MultiMeshCollisionTester(IEnumerable<IMeshCollisionTester> collection)
            :base(collection)
        {}

        public MultiMeshCollisionTester(int capacity)
            :base(capacity)
        {}

        public void AddRange(IEnumerable<Mesh> meshes)
        {
            AddRange(meshes.Select(item => new MeshCollisionTester(item)));
        }

        public IEnumerable<Mesh> Meshes
        {
            get { return this.SelectMany(x => x.Meshes); }
        }

        public MeshCollisionResult CollideWithRay(Vector3 point, Vector3 direction)
        {
            var results = new List<MeshCollisionResult>();
            foreach (var item in this)
            {
                results.Add(item.CollideWithRay(point, direction));
            }
            return MergeResults(point, results);
        }

        public MeshCollisionResult CollideWithLineSegment(Vector3 point, Vector3 direction, float distance)
        {
            var results = new List<MeshCollisionResult>();
            foreach (var item in this)
            {
                results.Add(item.CollideWithLineSegment(point, direction, distance));
            }
            return MergeResults(point, results);
        }

        public bool MeshesCollide(Mesh target)
        {
            throw new NotImplementedException();
        }

        private MeshCollisionResult MergeResults(Vector3 point, IEnumerable<MeshCollisionResult> results)
        {
            var ret = new MeshCollisionResult(new CollisionResult(), 0);
            float distance = -1;

            foreach (var res in results.Where(res => res != null && res.RayCollisionResult.HasCollided))
            {
                if (Math.Abs(distance - -1) < 0.001f)
                {
                    ret = res;
                    distance = point.Distance(res.RayCollisionResult.CollisionPoint);
                }
                else
                {
                    var tmp = point.Distance(res.RayCollisionResult.CollisionPoint);
                    if (tmp < distance)
                    {
                        distance = tmp;
                        ret = res;
                    }
                }
            }
            return ret;
        }
    }
}
