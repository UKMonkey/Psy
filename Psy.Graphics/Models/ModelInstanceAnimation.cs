using System;
using Psy.Core.EpicModel;

namespace Psy.Graphics.Models
{
    public class ModelInstanceAnimation : IComparable<ModelInstanceAnimation>
    {
        public float AnimationTime;
        public readonly AnimationType AnimationType;
        public bool Loop;
        public bool Running;

        public ModelInstanceAnimation(float animationTime, AnimationType animationType, bool loop)
        {
            AnimationTime = animationTime;
            AnimationType = animationType;
            Loop = loop;
            Running = true;
        }

        public int CompareTo(ModelInstanceAnimation other)
        {
            return (int)AnimationType - (int)other.AnimationType;
        }
    }
}