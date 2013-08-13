using SlimMath;

namespace Psy.Core.EpicModel
{
    public class ModelPartAnimState
    {
        public readonly ModelPart ModelPart;
        public Vector3 Rotation;
        public Vector3 Position;

        public ModelPartAnimState(ModelPart modelPart)
        {
            ModelPart = modelPart;
        }

        public ModelPartAnimState Clone()
        {
            return new ModelPartAnimState(ModelPart)
            {
                Position = Position,
                Rotation = Rotation
            };
        }

        public static ModelPartAnimState FromModelPart(ModelPart modelPart)
        {
            return new ModelPartAnimState(modelPart)
            {
                Position = modelPart.Position,
                Rotation = modelPart.Rotation
            };
        }

        public void Apply()
        {
            ModelPart.Position = Position;
            ModelPart.Rotation = Rotation;
        }

        /// <summary>
        /// Interpolate between this state and another.
        /// </summary>
        /// <param name="endFrame"></param>
        /// <param name="ratio"></param>
        public void Apply(ModelPartAnimState endFrame, double ratio)
        {
            var position = Vector3.SmoothStep(Position, endFrame.Position, (float) ratio);
            var rotation = Vector3.SmoothStep(Rotation, endFrame.Rotation, (float) ratio);

            ModelPart.Rotation = rotation;
            ModelPart.Position = position;
        }
    }
}