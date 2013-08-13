namespace Psy.Core.EpicModel
{
    public class PickVolumeResult
    {
        public CollisionVolume CollisionVolume;
        public int FaceIndex;
        public float DistanceFromCamera;

        public static readonly PickVolumeResult Nothing = new PickVolumeResult();
    }
}