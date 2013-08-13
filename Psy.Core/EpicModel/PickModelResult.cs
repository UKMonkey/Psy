namespace Psy.Core.EpicModel
{
    public class PickModelResult
    {
        public ModelPart ModelPart;
        public int FaceIndex;
        public float DistanceFromCamera;

        public static readonly PickModelResult Nothing = new PickModelResult();
    }
}