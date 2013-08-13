using System;
using System.Collections.Generic;
using System.Linq;

namespace Psy.Core.EpicModel
{
    public class Keyframe
    {
        private readonly Animation _animation;

        private float _time;
        public float Time
        {
            get { return _time; }
            set
            {
                if (Math.Abs(_time - value) > 0.001f)
                {
                    _time = value;
                    _animation.SortFrames();
                }
                else
                {
                    _time = value;
                }
            }
        }

        public List<AnchorAnimState> AnchorAnimStates { get; private set; }
        public List<ModelPartAnimState> ModelPartAnimStates { get; private set; }

        public Keyframe(Animation animation, float time)
        {
            _animation = animation;
            Time = time;
            ModelPartAnimStates = new List<ModelPartAnimState>();
            AnchorAnimStates = new List<AnchorAnimState>();
        }

        public void LoadFromKeyframe(Keyframe precedingFrame)
        {
            ModelPartAnimStates = new List<ModelPartAnimState>(precedingFrame.ModelPartAnimStates.Count);
            foreach (var other in precedingFrame.ModelPartAnimStates)
            {
                if (_animation.Ignores(other.ModelPart))
                {
                    continue;
                }

                var modelPartAnimState = other.Clone();
                ModelPartAnimStates.Add(modelPartAnimState);
            }

            AnchorAnimStates = new List<AnchorAnimState>(precedingFrame.AnchorAnimStates.Count);
            foreach (var other in precedingFrame.AnchorAnimStates)
            {
                if (_animation.Ignores(other.Anchor))
                {
                    continue;
                }

                var anchorAnimState = other.Clone();
                AnchorAnimStates.Add(anchorAnimState);
            }
        }

        public void LoadFromCurrentModelState(EpicModel model)
        {
            ModelPartAnimStates = new List<ModelPartAnimState>();
            foreach (var modelPart in model.ModelParts)
            {
                if (_animation.Ignores(modelPart))
                {
                    continue;
                }

                var modelPartAnimState = ModelPartAnimState.FromModelPart(modelPart);
                ModelPartAnimStates.Add(modelPartAnimState);
            }

            AnchorAnimStates = new List<AnchorAnimState>();
            foreach (var anchor in model.GetAnchorsForAnimation(_animation))
            {
                if (_animation.Ignores(anchor))
                {
                    continue;
                }

                var anchorAnimState = AnchorAnimState.FromAnchor(anchor);
                AnchorAnimStates.Add(anchorAnimState);
            }
        }

        public void Apply()
        {
            foreach (var modelPartAnimState in ModelPartAnimStates)
            {
                modelPartAnimState.Apply();
            }

            foreach (var anchorAnimState in AnchorAnimStates)
            {
                anchorAnimState.Apply();
            }
        }
    }
}