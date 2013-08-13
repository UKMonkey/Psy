using System;
using System.Collections.Generic;
using System.Linq;
using Psy.Core;
using Psy.Core.Collision;
using Psy.Core.EpicModel;
using Psy.Core.Logging;
using SlimMath;

namespace Psy.Graphics.Models
{
    public class ModelInstance
    {
        public class SubModel
        {
            private readonly ModelInstance _parent;
            public readonly int AnchorIndex;
            public readonly ModelInstance ModelInstance;

            internal SubModel(ModelInstance parent, int anchorIndex, ModelInstance modelInstance)
            {
                _parent = parent;
                AnchorIndex = anchorIndex;
                ModelInstance = modelInstance;
            }

            public Matrix GetWorldMatrix()
            {
                return _parent.GetAnchorMatrix(AnchorIndex);
            }
        }

        public readonly CompiledModel Model;
        public readonly List<MeshInstance> MeshInstances;

        private bool _updatedStatic;
        private readonly Vector3[] _collisionMeshVertices;
        private Cube _cube;
        private MeshBuilder<Mesh> _meshBuilder;
        private Mesh _nonAABB;
        private readonly List<ModelInstanceAnimation> _animations;
        private readonly List<SubModel> _subModels; 

        public IEnumerable<SubModel> SubModels
        {
            get { return _subModels; }
        }

        private float? _topZ;
        public float TopVertexZ
        {
            get
            {
                if (!_topZ.HasValue)
                {
                    var boundingBox = GetBoundingBox();
                    _topZ = boundingBox.Minimum.Z;
                    _farY = boundingBox.Maximum.Y;
                }
                return _topZ.Value;
            }
        }

        private float? _farY;
        public float FarY
        {
            get
            {
                if (!_farY.HasValue)
                {
                    var boundingBox = GetBoundingBox();
                    _topZ = boundingBox.Minimum.Z;
                    _farY = boundingBox.Maximum.Y;
                }
                return _farY.Value;
            }
        }

        private float? _radius;
        public readonly List<AnchorInstance> Anchors;

        public float Radius
        {
            get
            {
                if (!_radius.HasValue)
                {
                    _radius = GetBoundingSphere().Radius;
                }

                return _radius.Value;
            }
        }

        public ModelInstance(CompiledModel model, MaterialCache materialCache)
        {
            Model = model;

            MeshInstances = new List<MeshInstance>(Model.Meshes.Count);
            foreach (var modelMesh in Model.Meshes)
            {
                var textureName = materialCache[modelMesh.MaterialId].TextureName;
                var meshInstance = new MeshInstance(textureName, modelMesh);
                MeshInstances.Add(meshInstance);
            }

            _animations = new List<ModelInstanceAnimation>(5);
            _subModels = new List<SubModel>();

            _collisionMeshVertices = new Vector3[GetTotalVertices()];

            Anchors = new List<AnchorInstance>(model.Anchors.Count);
            foreach (var anchor in model.Anchors)
            {
                Anchors.Add(new AnchorInstance(anchor.Id, anchor.Name, anchor.Position, anchor.Rotation));
            }
        }

        /// <summary>
        /// Get absolute anchor position
        /// </summary>
        /// <param name="anchorName"></param>
        /// <returns></returns>
        public Vector3 GetAnchorPosition(string anchorName)
        {
            var anchorIndex = GetAnchorIndex(anchorName);
            return Anchors[anchorIndex].Position;
        }

        /// <summary>
        /// Get anchor position and rotation matrix for models attached to the anchor
        /// </summary>
        /// <param name="anchorName"></param>
        /// <returns></returns>
        public Matrix GetAnchorMatrix(string anchorName)
        {
            var anchorIndex = GetAnchorIndex(anchorName);
            return Anchors[anchorIndex].GetMatrix();
        }

        /// <summary>
        /// Returns true if this model has the specified anchor
        /// </summary>
        /// <param name="anchorName">Anchor name (case insensitive)</param>
        /// <returns></returns>
        public bool HasAnchor(string anchorName)
        {
            var index = GetAnchorIndex(anchorName);
            return index >= 0;
        }

        /// <summary>
        /// Add a sub model to this model. The ModelInstance will then be owned by this
        /// object.
        /// </summary>
        /// <param name="anchorName">Name of the anchor to attach to (case insensitive)</param>
        /// <param name="modelInstance"></param>
        public void AddSubModel(string anchorName, ModelInstance modelInstance)
        {
            var anchorIndex = GetAnchorIndex(anchorName);
            if (anchorIndex == -1)
            {
#if DEBUG
                throw new Exception(string.Format("Could not attach model `{0}` to `{1}` on anchor `{2}`", 
                    modelInstance.Model.Name, Model.Name, anchorName));
#else
                return;
#endif
            }

            _subModels.Add(new SubModel(this, anchorIndex, modelInstance));
        }

        /// <summary>
        /// Remove all submodels of a specific ModelInstance.
        /// </summary>
        /// <param name="modelInstance"></param>
        public void RemoveSubModels(ModelInstance modelInstance)
        {
            _subModels.RemoveAll(x => x.ModelInstance == modelInstance);
        }

        /// <summary>
        /// Remove all submodels attached to an anchor.
        /// </summary>
        /// <param name="anchorName">Anchor name (case insensitive)</param>
        public void RemoveSubModels(string anchorName)
        {
            var anchorIndex = GetAnchorIndex(anchorName);
            _subModels.RemoveAll(x => x.AnchorIndex == anchorIndex);
        }

        /// <summary>
        /// Remove all submodels.
        /// </summary>
        public void RemoveSubModels()
        {
            _subModels.Clear();
        }

        /// <summary>
        /// All attached submodels. Shared ModelInstances are returned for
        /// each submodel they are attached to.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelInstance> GetAttachedSubModels()
        {
            foreach (var attachedModel in _subModels)
            {
                yield return attachedModel.ModelInstance;
            }
        }

        /// <summary>
        /// All submodels for a particular anchor
        /// </summary>
        /// <param name="anchorName">Anchor name (case insensitive)</param>
        /// <returns></returns>
        public IEnumerable<ModelInstance> GetAttachedSubModels(string anchorName)
        {
            var anchorIndex = GetAnchorIndex(anchorName);
            foreach (var attachedModel in _subModels)
            {
                if (attachedModel.AnchorIndex == anchorIndex)
                {
                    yield return attachedModel.ModelInstance;
                }
            }
        }

        public Mesh GetCollisionMesh()
        {
            return _nonAABB;
        }

        public BoundingBox GetBoundingBox()
        {
            // todo: retain the bounding box for the frame
            
            var boundingBox = new BoundingBox();

            foreach (var mesh in Model.Meshes)
            {
                var meshBox = BoundingBox.FromPoints(mesh.Vertices);
                boundingBox = BoundingBox.Merge(boundingBox, meshBox);
            }

            return boundingBox;
        }

        public BoundingSphere GetBoundingSphere()
        {
            // todo: retain the bounding sphere for the frame

            var boundingSphere = new BoundingSphere();

            foreach (var mesh in Model.Meshes)
            {
                var meshSphere = BoundingSphere.FromPoints(mesh.Vertices);
                boundingSphere = BoundingSphere.Merge(boundingSphere, meshSphere);
            }

            return boundingSphere;
        }

        /// <summary>
        /// Run an animation. Multiple animations can run at the same time.
        /// </summary>
        /// <param name="animationType"></param>
        /// <param name="loop">Run once, or run forever</param>
        /// <param name="resume">Resume this animation if it was stopped previously.</param>
        public void RunAnimation(AnimationType animationType, bool loop, bool resume = true)
        {
            if (IsStatic())
            {
                return;
            }

            var animation = _animations.SingleOrDefault(x => x.AnimationType == animationType);

            if (animation != null)
            {
                animation.Loop = loop;
                animation.Running = true;
                if (!resume)
                {
                    animation.AnimationTime = 0;
                }
            }
            else
            {
                _animations.Add(new ModelInstanceAnimation(0, animationType, loop));
                _animations.Sort();
            }
        }

        /// <summary>
        /// Update this instance and all attached submodel instances.
        /// </summary>
        /// <param name="t">Previous frame delta</param>
        public void Update(float t)
        {
            if (IsStatic())
            {
                UpdateStatic();
                return;
            }

            foreach (var modelInstanceAnimation in _animations)
            {
                if (!modelInstanceAnimation.Running)
                {
                    continue;
                }

                var frame = GetCurrentFrame(modelInstanceAnimation);
                ApplyFrame(frame);

                modelInstanceAnimation.AnimationTime += t;

                var animation = Model.Animations[modelInstanceAnimation.AnimationType];

                var animationLength = animation.Frames.Count / 24.0f;

                if ((modelInstanceAnimation.AnimationTime >= animationLength) && 
                    (!modelInstanceAnimation.Loop))
                {
                    modelInstanceAnimation.Running = false;
                }
            }

            foreach (var subModel in _subModels)
            {
                subModel.ModelInstance.Update(t);
            }
        }

        public void StopAnimation(AnimationType animationType)
        {
            var animation = _animations.SingleOrDefault(x => x.AnimationType == animationType);
            if (animation != null)
            {
                animation.Running = false;
            }
        }

        private Matrix GetAnchorMatrix(int anchorIndex)
        {
            return Anchors[anchorIndex].GetMatrix();
        }

        private int GetAnchorIndex(string anchorName)
        {
            return Model.Anchors.FindIndex(x => x.Name.Equals(anchorName, StringComparison.InvariantCultureIgnoreCase));
        }

        private CompiledModel.Frame GetCurrentFrame(ModelInstanceAnimation modelInstanceAnimation)
        {
            var animation = Model.Animations[modelInstanceAnimation.AnimationType];

            var frameIndex = (modelInstanceAnimation.AnimationTime / (1.0 / 24.0));
            var loopedFrameIndex = (int)Math.Round(frameIndex) % animation.Frames.Count;

            var frame = animation.Frames[loopedFrameIndex];
            return frame;
        }

        private void UpdateStatic()
        {
            if (_updatedStatic)
                return;
            _updatedStatic = true;

            var frame = Model.Animations[AnimationType.None].Frames[0];

            ApplyFrame(frame);
        }

        private void ApplyFrame(CompiledModel.Frame frame)
        {
            var cmeshVertsIndex = 0;

            var meshCount = Model.Meshes.Count;
            for (var index = 0; index < meshCount; index++)
            {
                var mesh = Model.Meshes[index];
                var meshInstance = MeshInstances[index];

                var vertexCount = mesh.Vertices.Length;
                for (var i = 0; i < vertexCount; i++)
                {
                    var mpsIndex = mesh.VertexPivotIndex[i];
                    var modelPartState = frame.ModelPartStates[mpsIndex];

                    if (modelPartState != null)
                    {
                        var position = Vector3.Transform(mesh.Vertices[i], modelPartState.Rotation) + modelPartState.Position;
                        meshInstance.VertexBuffer[i].Position = new Vector4(position, 1);

                        _collisionMeshVertices[cmeshVertsIndex] = position;
                        cmeshVertsIndex++;
                    }
                }
            }

            var anchorCount = Model.Anchors.Count;
            for (var index = 0; index < anchorCount; index++)
            {
                var anchor = Anchors[index];

                var anchorState = frame.AnchorStates[index];

                if (anchorState != null)
                {
                    anchor.Position = anchorState.Position;
                    anchor.Rotation = anchorState.Rotation;
                }
            }

            _cube = new Cube(_collisionMeshVertices);

            _meshBuilder = new MeshBuilder<Mesh>(new Mesh());
            _meshBuilder.Rectangle(_cube.BottomFrontLeft, _cube.TopFrontLeft, _cube.TopFrontRight, _cube.BottomFrontRight);
            _meshBuilder.Rectangle(_cube.BottomFrontRight, _cube.TopFrontRight, _cube.TopBackRight, _cube.BottomBackRight);
            _meshBuilder.Rectangle(_cube.BottomBackRight, _cube.TopBackRight, _cube.TopBackLeft, _cube.BottomBackLeft);
            _meshBuilder.Rectangle(_cube.BottomBackLeft, _cube.TopBackLeft, _cube.TopFrontLeft, _cube.BottomFrontLeft);
            _meshBuilder.Rectangle(_cube.TopFrontLeft, _cube.TopBackLeft, _cube.TopBackRight, _cube.TopFrontRight);
            _meshBuilder.Rectangle(_cube.BottomBackLeft, _cube.BottomFrontLeft, _cube.BottomFrontRight, _cube.BottomBackRight);

            _nonAABB = _meshBuilder.GetMesh();
        }

        private int GetTotalVertices()
        {
            return MeshInstances.Sum(x => x.VertexBuffer.Length);
        }

        private bool IsStatic()
        {
            return !Model.Animations.ContainsKey(AnimationType.Walking);
        }
    }
}