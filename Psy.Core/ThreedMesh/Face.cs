using SlimMath;

namespace Psy.Core.ThreedMesh
{
    public class Face
    {
        public int MaterialId;
        public Triangle Vertex;
        public Vector3 FaceNormal;
        public Triangle TextureCoordinate;
    }
}