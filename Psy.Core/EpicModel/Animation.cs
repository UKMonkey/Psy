using System.Collections.Generic;
using System.Linq;

namespace Psy.Core.EpicModel
{
    public class Animation
    {
        private readonly EpicModel _epicModel;
        public readonly AnimationType AnimationType;
        private readonly List<Keyframe> _keyframes;

        public IEnumerable<Keyframe> Keyframes
        {
            get { return _keyframes; }
        }

        public float Duration
        {
            get { return _keyframes.Last().Time; }
        }

        public int KeyframeCount
        {
            get { return _keyframes.Count; }
        }


        private readonly List<ModelPart> _ignoredModelParts; 
        public IEnumerable<ModelPart> IgnoredModelParts
        {
            get { return _ignoredModelParts; }
        }

        public Animation(EpicModel epicModel, AnimationType animationType)
        {
            _epicModel = epicModel;
            AnimationType = animationType;
            _keyframes = new List<Keyframe>(10);
            _ignoredModelParts = new List<ModelPart>();
        }

        public bool HasInitialFrame()
        {
            return _keyframes[0].Time == 0.0f;
        }

        /// <summary>
        /// Create a new keyframe.
        /// </summary>
        /// <param name="precedingFrame"></param>
        public Keyframe AddFrame(Keyframe precedingFrame = null)
        {
            float frameTime;

            if (precedingFrame == null || _keyframes.Contains(precedingFrame))
            {
                frameTime = 0;
            }
            else
            {
                var frameIndex = _keyframes.IndexOf(precedingFrame);

                if (frameIndex == _keyframes.Count - 1)
                {
                    frameTime = precedingFrame.Time + 0.1f;
                }
                else
                {
                    var frameAfterPrecedingFrame = _keyframes[frameIndex + 1];
                    var timeDifference = frameAfterPrecedingFrame.Time - precedingFrame.Time;
                    frameTime = precedingFrame.Time + (timeDifference / 2.0f);
                }
            }

            var newKeyFrame = new Keyframe(this, frameTime);

            _keyframes.Add(newKeyFrame);

            if (precedingFrame != null)
            {
                newKeyFrame.LoadFromKeyframe(precedingFrame);
            }
            else
            {
                newKeyFrame.LoadFromCurrentModelState(_epicModel);
            }

            SortFrames();

            return newKeyFrame;
        }

        public Keyframe AddFrame(float time)
        {
            var frame = new Keyframe(this, time);
            _keyframes.Add(frame);
            SortFrames();
            return frame;
        }

        internal void SortFrames()
        {
            _keyframes.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        public void ApplyKeyFrame(int index)
        {
            if (index < 0 || index > _keyframes.Count)
            {
                return;
            }

            _keyframes[index].Apply();
        }

        public void MoveKeyFrame(int index, float position)
        {
            if (index < 0 || index > _keyframes.Count)
            {
                return;
            }

            _keyframes[index].Time = position;

            SortFrames();
        }

        public void SaveModelStateIntoKeyFrame(int index)
        {
            if (index < 0 || index > _keyframes.Count)
            {
                return;
            }

            _keyframes[index].LoadFromCurrentModelState(_epicModel);
        }

        public void DeleteKeyFrame(int index)
        {
            if (index < 0 || index > _keyframes.Count)
            {
                return;
            }

            _keyframes.RemoveAt(index);
            SortFrames();
        }

        /// <summary>
        /// Apply the animation on a model at a particular time in the animation.
        /// Not intended for running a full animation cycle.
        /// </summary>
        /// <param name="f"></param>
        public void ApplyAtTime(float f)
        {   
            var frame = _keyframes.LastOrDefault(x => x.Time <= f);

            if (frame == null)
                return;

            var frameIndex = _keyframes.IndexOf(frame);
            if (frameIndex >= _keyframes.Count-1)
            {
                frame.Apply();
                return;
            }

            var nextFrame = _keyframes[frameIndex + 1];


            var timeBetweenFrames = nextFrame.Time - frame.Time;
            var elapsedTimeSinceFirstFrame = f - frame.Time;

            var ratio = elapsedTimeSinceFirstFrame / timeBetweenFrames;

            foreach (var modelPart in _epicModel.ModelParts)
            {
                var startFrame = frame.ModelPartAnimStates.SingleOrDefault(x => x.ModelPart == modelPart);
                if (startFrame == null)
                    continue;

                var endFrame = nextFrame.ModelPartAnimStates.SingleOrDefault(x => x.ModelPart == modelPart);
                if (endFrame == null)
                {
                    startFrame.Apply();
                }
                else
                {
                    startFrame.Apply(endFrame, ratio);
                }
            }

            foreach (var anchor in _epicModel.GetAnchorsForAnimation(this))
            {
                var startFrame = frame.AnchorAnimStates.SingleOrDefault(x => x.Anchor == anchor);
                if (startFrame == null)
                    continue;

                var endFrame = nextFrame.AnchorAnimStates.SingleOrDefault(x => x.Anchor == anchor);
                if (endFrame == null)
                {
                    startFrame.Apply();
                }
                else
                {
                    startFrame.Apply(endFrame, ratio);
                }
            }
        }

        public void Ignore(ModelPart modelPart)
        {
            if (_ignoredModelParts.Contains(modelPart))
                return;

            _ignoredModelParts.Add(modelPart);

            foreach (var keyframe in Keyframes)
            {
                var modelPartAnimStateVictims = keyframe
                    .ModelPartAnimStates
                    .Where(modelPartAnimState => modelPartAnimState.ModelPart == modelPart)
                    .ToList();
                foreach (var modelPartAnimStateVictim in modelPartAnimStateVictims)
                {
                    keyframe.ModelPartAnimStates.Remove(modelPartAnimStateVictim);
                }

                var anchorAnimStateVictims = keyframe
                    .AnchorAnimStates
                    .Where(anchorAnimState => anchorAnimState.Anchor.ModelPart == modelPart)
                    .ToList();
                foreach (var anchorAnimStateVictim in anchorAnimStateVictims)
                {
                    keyframe.AnchorAnimStates.Remove(anchorAnimStateVictim);
                }
            }
        }

        public bool Ignores(ModelPart modelPart)
        {
            return _ignoredModelParts.Contains(modelPart);
        }

        public bool Ignores(Anchor anchor)
        {
            return _ignoredModelParts.Contains(anchor.ModelPart);
        }

        public void Include(ModelPart modelPart)
        {
            _ignoredModelParts.Remove(modelPart);
        }
    }
}