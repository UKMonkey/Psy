using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Psy.Core.Logging;
using SlimMath;

namespace Psy.Core.EpicModel.Serialization
{
    public class EpicModelReader
    {
        private readonly IMaterialTranslator _materialTranslator;
        private readonly BinaryReader _reader;
        private EpicModel _model;
        private Dictionary<Anchor, List<int>> _childIds;
        private ModelFormat _modelFormat;

        public EpicModelReader(IMaterialTranslator materialTranslator, BinaryReader reader)
        {
            _materialTranslator = materialTranslator;
            _reader = reader;
        }

        public EpicModel Read(string name)
        {
            _childIds = new Dictionary<Anchor, List<int>>(); 

            _model = new EpicModel(name);

            ReadHeader();
            ReadModelParts();
            ReadAnimations();

            foreach (var modelPart in _model.ModelParts)
            {
                foreach (var anchor in modelPart.Anchors)
                {
                    if (!_childIds.ContainsKey(anchor))
                        continue;

                    foreach (var i in _childIds[anchor])
                    {
                        var childModelPart = _model.ModelParts.Single(x => x.Id == i);
                        childModelPart.Pivot.SetParent(anchor, false);
                    }
                }
            }

            return _model;
        }

        private void ReadAnimations()
        {
            var animationCount = _reader.ReadInt32();
            for (var i = 0; i < animationCount; i++)
            {
                ReadAnimation();
            }
        }

        private void ReadAnimation()
        {
            var animationType = (AnimationType)_reader.ReadInt32();

            var animation = new Animation(_model, animationType);

            _model.Animations.Add(animation);

            if (_modelFormat >= ModelFormat.Emf3)
            {
                var ignoredModelPartsCount = _reader.ReadInt32();
                for (var i = 0; i < ignoredModelPartsCount; i++)
                {
                    var ignoredModelPartId = _reader.ReadInt32();
                    var modelPart = _model.ModelParts.Single(x => x.Id == ignoredModelPartId);
                    animation.Ignore(modelPart);
                }
            }

            var keyframeCount = _reader.ReadInt32();
            for (var i = 0; i < keyframeCount; i++)
            {
                ReadAnimationKeyframe(animation);
            }
        }

        private void ReadAnimationKeyframe(Animation animation)
        {
            var time = _reader.ReadSingle();

            var keyframe = animation.AddFrame(time);

            var modelPartAnimStateCount = _reader.ReadInt32();
            for (var i = 0; i < modelPartAnimStateCount; i++)
            {
                ReadModelPartAnimState(keyframe);
            }

            if (_modelFormat >= ModelFormat.Emf3)
            {
                var anchorAnimStateCount = _reader.ReadInt32();
                for (var i = 0; i < anchorAnimStateCount; i++)
                {
                    ReadAnchorAnimState(keyframe);
                }
            }
        }

        private void ReadAnchorAnimState(Keyframe keyframe)
        {
            var anchorId = _reader.ReadInt32();
            var rotation = _reader.ReadVector();

            var anchor = _model.GetAnchorById(anchorId);

            var malformedData = anchor == null;

            if (malformedData)
            {
                Logger.Write(string.Format("Failed to read anchor anim state: No such anchor with id `{0}` in this model", anchorId), LoggerLevel.Error);
                return;
            }

            var anchorAnimState = new AnchorAnimState(anchor, rotation);

            keyframe.AnchorAnimStates.Add(anchorAnimState);
        }

        private void ReadModelPartAnimState(Keyframe keyframe)
        {
            var modelPartId = _reader.ReadInt32();
            var position = _reader.ReadVector();
            var rotation = _reader.ReadVector();

            var modelPart = _model.ModelParts.Single(x => x.Id == modelPartId);

            var modelPartAnimState = new ModelPartAnimState(modelPart)
            {
                Position = position, 
                Rotation = rotation
            };

            keyframe.ModelPartAnimStates.Add(modelPartAnimState);
        }

        private void ReadModelParts()
        {
            var modelPartCount = _reader.ReadInt32();
            for (var i = 0; i < modelPartCount; i++)
            {
                ReadModelPart();
            }
        }

        private void ReadModelPart()
        {
            var id = _reader.ReadInt32();
            var modelPart = new ModelPart(id)
            {
                Name = _reader.ReadString(), 
                Position = _reader.ReadVector()
            };

            if (_modelFormat >= ModelFormat.Emf2)
            {
                modelPart.Rotation = _reader.ReadVector();
            }

            var materialName = _reader.ReadString();
            modelPart.MaterialId = _materialTranslator.Translate(materialName);

            ReadModelPartAnchors(modelPart);
            ReadModelPartFaces(modelPart);
            ReadModelPartVertices(modelPart);

            _model.ModelParts.Add(modelPart);
        }

        private void ReadModelPartVertices(ModelPart modelPart)
        {
            var vertexCount = _reader.ReadInt32();

            modelPart.Vertices = new Vector3[vertexCount];
            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = _reader.ReadVector();
                modelPart.Vertices[i] = vertex;
            }
        }

        private void ReadModelPartFaces(ModelPart modelPart)
        {
            var faceCount = _reader.ReadInt32();

            modelPart.Faces = new ModelPartFace[faceCount];
            for (var i = 0; i < faceCount; i++)
            {
                var face = ReadModelPartFace(modelPart);
                modelPart.Faces[i] = face;
            }
        }

        private ModelPartFace ReadModelPartFace(ModelPart modelPart)
        {
            var index = _reader.ReadInt32();

            var modelPartFace = new ModelPartFace(index, modelPart) {Colour = _reader.ReadColour()};

            var texCoordinateCount = _reader.ReadInt32();
            modelPartFace.TextureCoordinates = new Vector2[texCoordinateCount];
            for (var i = 0; i < texCoordinateCount; i++)
            {
                modelPartFace.TextureCoordinates[i] = _reader.ReadVectorXY();
            }

            var vertexIndexCount = _reader.ReadInt32();
            modelPartFace.VertexIndices = new int[vertexIndexCount];
            for (var i = 0; i < vertexIndexCount; i++)
            {
                modelPartFace.VertexIndices[i] = _reader.ReadInt32();
            }

            ReadModelPartFaceTriangles(modelPartFace);

            return modelPartFace;
        }

        private void ReadModelPartFaceTriangles(ModelPartFace modelPartFace)
        {
            var count = _reader.ReadInt32();
            modelPartFace.Triangles = new ModelTriangle[count];
            for (var i = 0; i < count; i++)
            {
                modelPartFace.Triangles[i] = ReadModelPartTriangle(modelPartFace);
            }
        }

        private ModelTriangle ReadModelPartTriangle(ModelPartFace modelPartFace)
        {
            var v1VertexIndex = _reader.ReadInt32();
            var v1TexCoordIndex = _reader.ReadInt32();

            var v2VertexIndex = _reader.ReadInt32();
            var v2TexCoordIndex = _reader.ReadInt32();

            var v3VertexIndex = _reader.ReadInt32();
            var v3TexCoordIndex = _reader.ReadInt32();

            return new ModelTriangle(
                v1VertexIndex, v2VertexIndex, v3VertexIndex, 
                modelPartFace, 
                v1TexCoordIndex, v2TexCoordIndex, v3TexCoordIndex);
        }

        private void ReadModelPartAnchors(ModelPart modelPart)
        {
            var anchorCount = _reader.ReadInt32();
            modelPart.Anchors.Clear();
            for (int i = 0; i < anchorCount; i++)
            {
                modelPart.Anchors.Add(ReadAnchor(modelPart));
            }
        }

        private Anchor ReadAnchor(ModelPart modelPart)
        {
            int? id = null;

            if (_modelFormat >= ModelFormat.Emf4)
            {
                id = _reader.ReadInt32();
            }

            var position = _reader.ReadVector();
            var name = _reader.ReadString();

            var rotation = new Vector3();
            if (_modelFormat >= ModelFormat.Emf3)
            {
                rotation = _reader.ReadVector();
            }

            var anchor = new Anchor(modelPart, position, name)
            {
                Rotation = rotation
            };

            if (id.HasValue)
            {
                anchor.Id = id.Value;
            }

            var childCount = _reader.ReadInt32();

            for (var i = 0; i < childCount; i++)
            {
                if (!_childIds.ContainsKey(anchor))
                {
                    _childIds[anchor] = new List<int>();
                }

                var childId = _reader.ReadInt32();
                _childIds[anchor].Add(childId);
            }
            return anchor;
        }

        private void ReadHeader()
        {
            var headerStr = new string(_reader.ReadChars(4));
            if (headerStr == "EMF1")
            {
                _modelFormat = ModelFormat.Emf1;
            }
            else if (headerStr == "EMF2")
            {
                _modelFormat = ModelFormat.Emf2;
            }
            else if (headerStr == "EMF3")
            {
                _modelFormat = ModelFormat.Emf3;
            }
            else if (headerStr == "EMF4")
            {
                _modelFormat = ModelFormat.Emf4;
            }
            else
            {
                throw new Exception(string.Format("Expected model to begin with header `EMF1` or `EMF2`. Got `{0}` instead", headerStr));
            }
        }
    }

    public enum ModelFormat
    {
        Emf1,

        /// <summary>
        /// modelPart now has a rotation
        /// </summary>
        Emf2,

        /// <summary>
        /// - anchor now has a rotation
        /// - animation can have ignored model parts
        /// </summary>
        Emf3,

        /// <summary>
        /// - anchor now has an id (oops)
        /// </summary>
        Emf4
       
    }
}