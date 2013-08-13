using SlimMath;

namespace Psy.Core.EpicModel
{
    public class ModelPartFace
    {
        public readonly int Index;
        public Vector2[] TextureCoordinates;
        public int[] VertexIndices;
        public ModelTriangle[] Triangles;
        public Color4 Colour;
        public readonly ModelPart ModelPart;

        public ModelPartFace(int index, ModelPart modelPart)
        {
            Index = index;
            Colour = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
            ModelPart = modelPart;
        }

        public static ModelPartFace CreateSquare(ModelPart modelPart, int index)
        {
            return new ModelPartFace(index, modelPart)
            {
                VertexIndices = new int[4],
                TextureCoordinates = new[]
                {
                    new Vector2(0, 1), 
                    new Vector2(0, 0), 
                    new Vector2(1, 0), 
                    new Vector2(1, 1), 
                },
                Colour = new Color4(1.0f, 1.0f, 1.0f, 1.0f),
                Triangles = new ModelTriangle[2]
            };
        }

        public void Translate(Vector3 vector)
        {
            foreach (var vertexIndex in VertexIndices)
            {
                ModelPart.Vertices[vertexIndex] = ModelPart.Vertices[vertexIndex] + vector;
            }
        }

        public void Resize(Vector3 diffVector)
        {
            var middle = GetMiddle();

            foreach (var vertexIndex in VertexIndices)
            {
                var vector = ModelPart.Vertices[vertexIndex];

                var diff = middle - vector;
                diff = new Vector3(diff.X * diffVector.X, diff.Y * diffVector.Y, diff.Z * diffVector.Z);

                ModelPart.Vertices[vertexIndex] = vector + diff;
            }
        }

        private Vector3 GetMiddle()
        {
            var middle = new Vector3();

            foreach (var vertexIndex in VertexIndices)
            {
                middle = middle + ModelPart.Vertices[vertexIndex];
            }

            middle = middle/VertexIndices.Length;
            return middle;
        }

        public void Rotate(Vector3 diffVector)
        {
            var rotationMatrix = Matrix.RotationYawPitchRoll(diffVector.Y, diffVector.X, diffVector.Z);

            var middle = GetMiddle();

            foreach (var vertexIndex in VertexIndices)
            {
                var vector = ModelPart.Vertices[vertexIndex];
                vector -= middle;
                vector = Vector3.TransformCoordinate(vector, rotationMatrix);

                vector += middle;

                ModelPart.Vertices[vertexIndex] = vector;
            }
        }

        public ModelPartFace Clone(ModelPart parent)
        {
            var clone = new ModelPartFace(Index, parent)
            {
                Colour = Colour,
                TextureCoordinates = new Vector2[TextureCoordinates.Length]
            };

            for (var i = 0; i < TextureCoordinates.Length; i++)
            {
                clone.TextureCoordinates[i] = TextureCoordinates[i];
            }

            clone.Triangles = new ModelTriangle[Triangles.Length];
            for (var i = 0; i < Triangles.Length; i++)
            {
                clone.Triangles[i] = Triangles[i].Clone(clone);
            }

            clone.VertexIndices = new int[VertexIndices.Length];
            for (var i = 0; i < VertexIndices.Length; i++)
            {
                clone.VertexIndices[i] = VertexIndices[i];
            }

            return clone;
        }

        public void RotateTextureCoordinatesCounterClockwise()
        {
            TextureCoordinates = TextureCoordinates.Cycle(1);
        }

        public void RotateTextureCoordinatesClockwise()
        {
            TextureCoordinates = TextureCoordinates.Cycle(-1);
        }
    }
}