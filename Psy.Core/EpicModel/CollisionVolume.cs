using System;
using SlimMath;

namespace Psy.Core.EpicModel
{
    public class CollisionVolume
    {
        private static int _nextId = 1;

        public int Id;
        public string Name;
        public Vector3 Size;
        public Vector3 Position;
        public Vector3 Rotation;
        public CollisionVolumeType CollisionVolumeType;

        public CollisionVolume(string name, Vector3 size, Vector3 position,
                               Vector3 rotation, CollisionVolumeType collisionVolumeType)
        {
            Id = _nextId;
            _nextId++;
            Name = name;
            Size = size;
            Position = position;
            Rotation = rotation;
            CollisionVolumeType = collisionVolumeType;
        }

        private Quaternion GetRotation()
        {
            return Quaternion.RotationYawPitchRoll(Rotation.Y, -Rotation.X, Rotation.Z);
        }

        private Matrix GetRotationMatrix()
        {
            var quat = GetRotation();
            var rotationMatrix = Matrix.RotationQuaternion(quat);
            return rotationMatrix;
        }

        public Matrix GetAbsoluteMatrix()
        {
            return  GetRotationMatrix() * Matrix.Translation(Position);
        }

        private Ray GetModelRay(Ray cameraRay)
        {
            var matrix = GetAbsoluteMatrix();
            var invWorldMatrix = Matrix.Invert(matrix);

            var rayDir = Vector3.TransformNormal(cameraRay.Direction, invWorldMatrix);
            var rayOrigin = Vector3.TransformCoordinate(cameraRay.Position, invWorldMatrix);

            var rayDir3 = new Vector3(rayDir.X, rayDir.Y, rayDir.Z);
            var rayOrigin3 = new Vector3(rayOrigin.X, rayOrigin.Y, rayOrigin.Z);

            var ray3 = new Ray(rayOrigin3, rayDir3);
            return ray3;
        }

        public PickVolumeResult IntersectsWithRay(Ray cameraRay)
        {
            var ray3 = GetModelRay(cameraRay);

            int? selectedFace = null;

            var minDistance = 1000f;

            if (CollisionVolumeType == CollisionVolumeType.Cuboid)
            {
                var boundingBox = new BoundingBox(-Size.X, -Size.Y, -Size.Z, Size.X, Size.Y, Size.Z);
            }
            else if (CollisionVolumeType == CollisionVolumeType.Capsule)
            {

            }
            else
            {
                throw new Exception(string.Format("Unknown collision volume type {0}", CollisionVolumeType));
            }

            /*
            var triangles = GetTriangles();

            foreach (var triangle in triangles)
            {
                float distance;

                if (!triangle.IsIntersectedBy(ray3, out distance))
                    continue;

                if (distance > minDistance)
                    continue;

                selectedFace = triangle.Face.Index;
                minDistance = distance;
            }
            */
            if (selectedFace == null)
            {
                return PickVolumeResult.Nothing;
            }

            return new PickVolumeResult
            {
                DistanceFromCamera = minDistance,
                CollisionVolume = this,
                FaceIndex = (int)selectedFace
            };
        }
    }
}