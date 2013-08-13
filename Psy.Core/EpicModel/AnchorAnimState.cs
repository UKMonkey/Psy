using SlimMath;

namespace Psy.Core.EpicModel
{
    public class AnchorAnimState
    {
        public readonly Anchor Anchor;
        public Vector3 Rotation;

        public AnchorAnimState(Anchor anchor, Vector3 rotation)
        {
            Anchor = anchor;
            Rotation = rotation;
        }

        public void Apply()
        {
            Anchor.Rotation = Rotation;
        }

        public void Apply(AnchorAnimState endFrame, double ratio)
        {
            var rotation = Vector3.SmoothStep(Rotation, endFrame.Rotation, (float) ratio);
            Anchor.Rotation = rotation;
        }

        public AnchorAnimState Clone()
        {
            return new AnchorAnimState(Anchor, Rotation);
        }

        public static AnchorAnimState FromAnchor(Anchor anchor)
        {
            return new AnchorAnimState(anchor, anchor.Rotation);
        }
    }
}