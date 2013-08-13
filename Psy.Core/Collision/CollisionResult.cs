using SlimMath;

namespace Psy.Core.Collision
{
    public class CollisionResult
    {
        public bool HasCollided { get; set; }
        public Vector3 CollisionPoint { get; set; }
        public Mesh CollisionMesh { get; set; }
    }
}