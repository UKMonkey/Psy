using SlimMath;

namespace Psy.Core.EpicModel
{
    public class ModelTriangle
    {
        public readonly ModelPartFace Face;
        public readonly int V1VertexIndex;
        public readonly int V2VertexIndex;
        public readonly int V3VertexIndex;

        public readonly int V1TexCoordIndex;
        public readonly int V2TexCoordIndex;
        public readonly int V3TexCoordIndex;

        public ModelTriangle(int v1VertexIndex, int v2VertexIndex, int v3VertexIndex, ModelPartFace face, 
            int v1TexCoordIndex, int v2TexCoordIndex, int v3TexCoordIndex)
        {
            Face = face;
            V1VertexIndex = v1VertexIndex;
            V2VertexIndex = v2VertexIndex;
            V3VertexIndex = v3VertexIndex;
            V1TexCoordIndex = v1TexCoordIndex;
            V2TexCoordIndex = v2TexCoordIndex;
            V3TexCoordIndex = v3TexCoordIndex;
        }

        public bool IsIntersectedBy(Ray ray3, out float distance)
        {
            var v1 = Face.ModelPart.Vertices[V1VertexIndex];
            var v2 = Face.ModelPart.Vertices[V2VertexIndex];
            var v3 = Face.ModelPart.Vertices[V3VertexIndex];

            return ray3.Intersects(ref v1, ref v2, ref v3, out distance);
        }

        public Vector3 V1 { get { return Face.ModelPart.Vertices[V1VertexIndex]; } }
        public Vector3 V2 { get { return Face.ModelPart.Vertices[V2VertexIndex]; } }
        public Vector3 V3 { get { return Face.ModelPart.Vertices[V3VertexIndex]; } }

        public ModelTriangle Clone(ModelPartFace parent)
        {
            var clone = new ModelTriangle(V1VertexIndex, V2VertexIndex, V3VertexIndex, parent, V1TexCoordIndex,
                                          V2TexCoordIndex, V3TexCoordIndex);

            return clone;
        }
    }
}