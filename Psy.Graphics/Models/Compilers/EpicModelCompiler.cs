using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Psy.Core;
using Psy.Core.EpicModel;
using Psy.Core.Logging;

namespace Psy.Graphics.Models.Compilers
{
    public class EpicModelCompiler
    {
        private const int EpicModelFrameRate = 24;

        private readonly EpicModel _epicModel;
        private CompiledModel _compiledModel;
        private List<ModelPart> _modelPartsInMaterialOrder;
        private int _meshCount;

        public EpicModelCompiler(EpicModel epicModel)
        {
            _epicModel = epicModel;
        }

        public CompiledModel Compile()
        {
            _compiledModel = new CompiledModel(_epicModel.Name);

            LoadModelStructure();
            LoadAnchors();
            LoadModelData();
            CalculateAnimations();

            return _compiledModel;
        }

        private void LoadAnchors()
        {
            var anchors = _epicModel.GetAnchors().ToList();

            Logger.Write(string.Format("Model comprises of {0} anchors", anchors.Count));

            foreach (var anchor in anchors)
            {
                _compiledModel.Anchors.Add(
                    new CompiledModel.Anchor(
                        anchor.Id, 
                        anchor.Name, 
                        anchor.GetAbsolutePosition(), 
                        anchor.GetAbsoluteRotation()));
            }
        }

        private void LoadModelStructure()
        {
            _modelPartsInMaterialOrder = new List<ModelPart>(_epicModel.ModelParts);
            _modelPartsInMaterialOrder.Sort((x, y) => x.MaterialId.CompareTo(y.MaterialId));

            _meshCount = 0;

            var meshVertexCount = 0;

            foreach (var grouping in _modelPartsInMaterialOrder.GroupBy(x => x.MaterialId))
            {
                var materialId = grouping.Key;

                meshVertexCount +=
                    grouping.Sum(modelPart =>
                        modelPart.Faces.Sum(modelPartFace => modelPartFace.Triangles.Length * 3));
                

                _compiledModel.Meshes.Add(new CompiledModel.Mesh(meshVertexCount, materialId));
                Logger.Write(string.Format("Mesh #{0} comprising of {1} vertices for material {2}", _meshCount, meshVertexCount, materialId));
                _meshCount++;
                meshVertexCount = 0;
            }

            Logger.Write(string.Format("Model comprising of {0} meshes", _meshCount));
        }

        private void LoadModelData()
        {
            var i = 0;
            var t = 0;
            var previousMaterialId = -1;

            foreach (var mpIo in _modelPartsInMaterialOrder.IndexOver())
            {
                var modelPart = mpIo.Value;
                var index = mpIo.Index;

                var mesh = _compiledModel.Meshes.Single(x => x.MaterialId == modelPart.MaterialId);

                if (previousMaterialId != modelPart.MaterialId)
                {
                    i = 0;
                    t = 0;
                    previousMaterialId = modelPart.MaterialId;
                }

                foreach (var modelPartFace in modelPart.Faces)
                {

                    foreach (var triangle in modelPartFace.Triangles)
                    {
                        mesh.VertexPivotIndex[i] = index;
                        mesh.VertexPivotIndex[i+1] = index;
                        mesh.VertexPivotIndex[i+2] = index;

                        mesh.Vertices[i] = triangle.V1;
                        mesh.Vertices[i+1] = triangle.V2;
                        mesh.Vertices[i+2] = triangle.V3;

                        mesh.TextureCoordinateBuffer[i] = modelPartFace.TextureCoordinates[triangle.V1TexCoordIndex];
                        mesh.TextureCoordinateBuffer[i+1] = modelPartFace.TextureCoordinates[triangle.V2TexCoordIndex];
                        mesh.TextureCoordinateBuffer[i+2] = modelPartFace.TextureCoordinates[triangle.V3TexCoordIndex];

                        mesh.Triangles[t] = new CompiledModel.Triangle
                        {
                            Vertex1Index = i,
                            Vertex2Index = i+1,
                            Vertex3Index = i+2,
                        };

                        i += 3;
                        t++;
                    }
                }
            }
        }

        private void CalculateDefaultPose()
        {
            var defaultPoseAnimation = new CompiledModel.Animation(1, 0);

            var frame = new CompiledModel.Frame(_modelPartsInMaterialOrder.Count);

            foreach (var modelPart in _modelPartsInMaterialOrder)
            {
                frame.ModelPartStates.Add(new CompiledModel.ModelPartState(modelPart.GetAbsolutePosition(), modelPart.GetAbsoluteRotation()));

                foreach (var anchor in modelPart.Anchors)
                {
                    frame.AnchorStates.Add(new CompiledModel.AnchorState(anchor.GetAbsolutePosition(), anchor.GetAbsoluteRotation()));
                }
            }

            defaultPoseAnimation.Frames.Add(frame);

            _compiledModel.Animations[AnimationType.None] = defaultPoseAnimation;
        }

        private void CalculateAnimations()
        {
            CalculateDefaultPose();

            var expectedAnchorCount = _epicModel.GetAnchors().Count();

            foreach (var animation in _epicModel.Animations)
            {
                if (!animation.HasInitialFrame())
                {
                    throw new Exception("Animation must have frame at t=0");
                }

                var duration = animation.Duration;
                var frameCount = (int) (EpicModelFrameRate * duration);

                Logger.Write(string.Format(
                    "Compiling animation `{0}` - Duration: {1:0.00} Frames: {2}", 
                    animation.AnimationType, duration, frameCount));

                var compiledAnimation = new CompiledModel.Animation(frameCount, duration);

                var frameTime = 0.0f;
                while (frameTime < duration)
                {
                    animation.ApplyAtTime(frameTime);
                    var frame = new CompiledModel.Frame(_modelPartsInMaterialOrder.Count);

                    // Calculate model part states
                    foreach (var modelPart in _modelPartsInMaterialOrder)
                    {
                        if (!animation.Ignores(modelPart))
                        {
                            var modelPartState = new CompiledModel.ModelPartState(
                                modelPart.GetAbsolutePosition(), modelPart.GetAbsoluteRotation());

                            frame.ModelPartStates.Add(modelPartState);
                        }
                        else
                        {
                            frame.ModelPartStates.Add(null);
                        }
                    }

                    // Calculate anchor states
                    foreach (var modelPart in _epicModel.ModelParts)
                    {
                        if (!animation.Ignores(modelPart))
                        {
                            foreach (var anchor in modelPart.Anchors)
                            {
                                var anchorState = new CompiledModel.AnchorState(
                                    anchor.GetAbsolutePosition(), anchor.GetAbsoluteRotation());

                                frame.AnchorStates.Add(anchorState);
                            }
                        }
                        else
                        {
                            foreach (var anchor in modelPart.Anchors)
                            {
                                frame.AnchorStates.Add(null);
                            }
                        }
                    }

                    Debug.Assert(frame.AnchorStates.Count == expectedAnchorCount,
                                 "Compiled animation frame has incorrect number of anchors");

                    compiledAnimation.Frames.Add(frame);

                    frameTime += 1.0f / EpicModelFrameRate;
                }

                _compiledModel.Animations[animation.AnimationType] = compiledAnimation;
            }
        }
    }
}