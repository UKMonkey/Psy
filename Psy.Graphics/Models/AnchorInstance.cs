using Psy.Core.Logging;
using SlimMath;

namespace Psy.Graphics.Models
{
    public class AnchorInstance
    {
        public int Id;
        public string Name;
        public Quaternion Rotation;
        public Vector3 Position;

        public AnchorInstance(int id, string name, Vector3 position, Quaternion rotation)
        {
            Id = id;
            Name = name;
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Get rotation and translation matrix
        /// </summary>
        /// <returns></returns>
        public Matrix GetMatrix()
        {
            var rotationMatrix = Matrix.RotationQuaternion(Rotation);
            var translationMatrix = Matrix.Translation(Position);

            return rotationMatrix * translationMatrix;
        }
    }
}