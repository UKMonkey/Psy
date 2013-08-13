using System.IO;
using System.Linq;

namespace Psy.Core.EpicModel.Serialization
{
    public class EpicModelWriter
    {
        private readonly IMaterialTranslator _materialTranslator;

        public EpicModelWriter(IMaterialTranslator materialTranslator)
        {
            _materialTranslator = materialTranslator;
        }

        public void Write(BinaryWriter writer, EpicModel model)
        {
            WriteHeader(writer);
            WriteModelParts(writer, model);
            WriteAnimations(writer, model);
        }

        private static void WriteAnimations(BinaryWriter writer, EpicModel model)
        {
            writer.Write(model.Animations.Count);
            foreach (var animation in model.Animations)
            {
                WriteAnimation(writer, animation);
            }
        }

        private static void WriteAnimation(BinaryWriter writer, Animation animation)
        {
            writer.Write((int)animation.AnimationType);

            writer.Write(animation.IgnoredModelParts.Count());
            foreach (var ignoredModelPart in animation.IgnoredModelParts)
            {
                writer.Write(ignoredModelPart.Id);
            }

            writer.Write(animation.KeyframeCount);
            foreach (var keyframe in animation.Keyframes)
            {
                WriteKeyframe(writer, keyframe);
            }
        }

        private static void WriteKeyframe(BinaryWriter writer, Keyframe keyframe)
        {
            writer.Write(keyframe.Time);
            
            writer.Write(keyframe.ModelPartAnimStates.Count);
            foreach (var modelPartAnimState in keyframe.ModelPartAnimStates)
            {
                WriteModelPartAnimState(writer, modelPartAnimState);
            }

            writer.Write(keyframe.AnchorAnimStates.Count);
            foreach (var anchorAnimState in keyframe.AnchorAnimStates)
            {
                WriteAnchorAnimState(writer, anchorAnimState);
            }
        }

        private static void WriteAnchorAnimState(BinaryWriter writer, AnchorAnimState anchorAnimState)
        {
            writer.Write(anchorAnimState.Anchor.Id);
            writer.Write(anchorAnimState.Rotation);
        }

        private static void WriteModelPartAnimState(BinaryWriter writer, ModelPartAnimState modelPartAnimState)
        {
            writer.Write(modelPartAnimState.ModelPart.Id);
            writer.Write(modelPartAnimState.Position);
            writer.Write(modelPartAnimState.Rotation);
        }

        private void WriteModelParts(BinaryWriter writer, EpicModel model)
        {
            writer.Write(model.ModelParts.Count);
            foreach (var modelPart in model.ModelParts)
            {
                WriteModelPart(writer, modelPart);
            }
        }

        private void WriteModelPart(BinaryWriter writer, ModelPart modelPart)
        {
            writer.Write(modelPart.Id);
            writer.Write(modelPart.Name);
            writer.Write(modelPart.Position);
            writer.Write(modelPart.Rotation); // new for EMF2

            writer.Write(_materialTranslator.Translate(modelPart.MaterialId));

            WriteModelPartAnchors(writer, modelPart);
            WriteModelPartFaces(writer, modelPart);
            WriteModelPartVertices(writer, modelPart);

        }

        private static void WriteModelPartAnchors(BinaryWriter writer, ModelPart modelPart)
        {
            writer.Write(modelPart.Anchors.Count);
            foreach (var anchor in modelPart.Anchors)
            {
                WriteAnchor(writer, anchor);
            }
        }

        private static void WriteAnchor(BinaryWriter writer, Anchor anchor)
        {
            writer.Write(anchor.Id);
            writer.Write(anchor.Position);
            writer.Write(anchor.Name);
            writer.Write(anchor.Rotation);

            writer.Write(anchor.Children.Count);
            foreach (var child in anchor.Children)
            {
                writer.Write(child.Id);
            }
        }

        private static void WriteModelPartFaces(BinaryWriter writer, ModelPart modelPart)
        {
            writer.Write(modelPart.Faces.Length);
            foreach (var modelPartFace in modelPart.Faces)
            {
                WriteModelPartFace(writer, modelPartFace);
            }
        }

        private static void WriteModelPartFace(BinaryWriter writer, ModelPartFace modelPartFace)
        {
            writer.Write(modelPartFace.Index);
            writer.WriteColour(modelPartFace.Colour);
            
            writer.Write(modelPartFace.TextureCoordinates.Length);
            foreach (var textureCoordinate in modelPartFace.TextureCoordinates)
            {
                writer.WriteVectorXY(textureCoordinate);
            }

            writer.Write(modelPartFace.VertexIndices.Length);
            foreach (var vertexIndex in modelPartFace.VertexIndices)
            {
                writer.Write(vertexIndex);
            }

            WriteModelPartFaceTriangles(writer, modelPartFace);
        }

        private static void WriteModelPartFaceTriangles(BinaryWriter writer, ModelPartFace modelPartFace)
        {
            writer.Write(modelPartFace.Triangles.Length);
            foreach (var modelTriangle in modelPartFace.Triangles)
            {
                WriteModelPartTriangle(writer, modelTriangle);
            }
        }

        private static void WriteModelPartTriangle(BinaryWriter writer, ModelTriangle modelTriangle)
        {
            writer.Write(modelTriangle.V1VertexIndex);
            writer.Write(modelTriangle.V1TexCoordIndex);

            writer.Write(modelTriangle.V2VertexIndex);
            writer.Write(modelTriangle.V2TexCoordIndex);

            writer.Write(modelTriangle.V3VertexIndex);
            writer.Write(modelTriangle.V3TexCoordIndex);
        }

        private static void WriteModelPartVertices(BinaryWriter writer, ModelPart modelPart)
        {
            writer.Write(modelPart.Vertices.Length);
            foreach (var vector3 in modelPart.Vertices)
            {
                writer.Write(vector3);
            }
        }

        private static void WriteHeader(BinaryWriter writer)
        {
            writer.Write(new[] {'E', 'M', 'F', '4'});
        }
    }
}