using System.Collections.Generic;
using SlimMath;

namespace Psy.Core.Collision
{
    public interface IMeshCollisionTester
    {
        IEnumerable<Mesh> Meshes { get; }

        MeshCollisionResult CollideWithRay(Vector3 point, Vector3 direction);
        MeshCollisionResult CollideWithLineSegment(Vector3 point, Vector3 direction, float distance);
    }
}
