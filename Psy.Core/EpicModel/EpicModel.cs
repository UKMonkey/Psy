using System.Collections.Generic;
using System.Linq;
using SlimMath;

namespace Psy.Core.EpicModel
{
    public class EpicModel
    {
        public readonly List<ModelPart> ModelParts;
        public readonly List<Animation> Animations;
        public readonly List<CollisionVolume> CollisionVolumes; 
        public readonly string Name; 

        private BoundingBox? _boundingBox;

        public BoundingBox GetBoundingBox()
        {
            if (_boundingBox != null)
            {
                return _boundingBox.Value;
            }

            var boundingBox = new BoundingBox();

            foreach (var modelPart in ModelParts)
            {
                boundingBox = BoundingBox.Merge(boundingBox, modelPart.GetBoundingBox());
            }

            _boundingBox = boundingBox;

            return _boundingBox.Value;
        }

        public EpicModel()
        {
            Name = "unnamed_model";
            ModelParts = new List<ModelPart>();
            Animations = new List<Animation>();
            CollisionVolumes = new List<CollisionVolume>();
        }

        public EpicModel(string name) : this()
        {
            Name = name;
        }

        public PickModelResult PickModelPart(Ray cameraRay)
        {
            var cameraDist = float.MaxValue;
            var result = PickModelResult.Nothing;

            foreach (var modelPart in ModelParts)
            {
                var test = modelPart.IntersectsWithRay(cameraRay);

                if (test == PickModelResult.Nothing)
                    continue;

                if (test.DistanceFromCamera > cameraDist)
                    continue;

                result = test;
                cameraDist = test.DistanceFromCamera;
            }

            return result;
        }

        public void RemoveModelPart(ModelPart modelPart)
        {
            if (ModelParts.Contains(modelPart))
            {
                ModelParts.Remove(modelPart);
            }
        }

        public PickAnchorResult PickAnchor(Ray cameraRay)
        {
            foreach (var modelPart in ModelParts)
            {
                var result = modelPart.GetIntersectingAnchor(cameraRay);
                if (result != PickAnchorResult.Nothing)
                {
                    return result;
                }
            }

            return PickAnchorResult.Nothing;
        }

        /// <summary>
        /// Get an animation for this model. If the animation type does not
        /// exist then it created.
        /// </summary>
        /// <param name="animationType"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public Animation GetAnimation(AnimationType animationType, bool autoCreate = false)
        {
            var animation = Animations.SingleOrDefault(x => x.AnimationType == animationType);

            if (animation != null)
                return animation;

            if (!autoCreate)
                return null;

            animation = new Animation(this, animationType);
            Animations.Add(animation);

            return animation;
        }

        public IEnumerable<Anchor> GetAnchors()
        {
            foreach (var modelPart in ModelParts)
            {
                foreach (var anchor in modelPart.Anchors)
                {
                    yield return anchor;
                }
            }
        }

        public Anchor GetAnchorById(int anchorId)
        {
            foreach (var modelPart in ModelParts)
            {
                var anchor = modelPart.Anchors.SingleOrDefault(x => x.Id == anchorId);
                if (anchor != null)
                {
                    return anchor;
                }
            }
            return null;
        }

        public IEnumerable<Anchor> GetAnchorsForAnimation(Animation animation)
        {
            return ModelParts
                .Where(x => !animation.IgnoredModelParts.Contains(x))
                .SelectMany(modelPart => modelPart.Anchors);
        }
    }

    public class PickAnchorResult
    {
        public Anchor Anchor;

        public static readonly PickAnchorResult Nothing = new PickAnchorResult();
    }
}