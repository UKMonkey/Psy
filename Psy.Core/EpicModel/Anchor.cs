using System;
using System.Collections.Generic;
using SlimMath;

namespace Psy.Core.EpicModel
{
    public class Anchor
    {
        private const string PivotName = "PIVOT";

        private static int _nextId = 1;

        public int Id;

        public readonly ModelPart ModelPart;
        public Anchor Parent { get; private set; }

        /// <summary>
        /// Position relative to the ModelPart
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Rotation for attachments
        /// </summary>
        public Vector3 Rotation { get; set; }

        public readonly List<ModelPart> Children;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == PivotName)
                {
                    throw new Exception("Cannot rename a pivot anchor. It answers only to `PIVOT`.");
                }
                _name = value;
            }
        }

        public bool IsPivot
        {
            get { return Name == PivotName; }
        }

        public bool HasParent
        {
            get { return Parent != null; }
        }

        public bool HasChildren
        {
            get { return Children.Count > 0; }
        }

        public Anchor(ModelPart modelPart, Vector3 position, string name)
        {
            Id = _nextId++;

            Name = name;
            ModelPart = modelPart;
            Position = position;
            Children = new List<ModelPart>();
        }

        public void SetParent(Anchor parent, bool translate=true)
        {
            if (!IsPivot)
            {
                throw new Exception("SetParent can only be called on a Pivot.");
            }

            var absolutePosition = GetAbsolutePosition();

            if (Parent != null)
            {
                Parent.Children.Remove(ModelPart);
            }

            Parent = parent;
            Parent.Children.Add(ModelPart);

            if (translate)
            {
                
                ModelPart.Position = absolutePosition - parent.GetAbsolutePosition();                
            }
        }

        public void DetachFromParent()
        {
            Parent.Children.Remove(ModelPart);
            Parent = null;
        }

        public void RemoveChild(ModelPart modelPart)
        {
            if (!Children.Contains(modelPart))
            {
                throw new Exception("Cannot remove modelPart from this Anchor. This Anchor is not a parent of the ModelPart.");
            }

            var childAnchor = modelPart.Pivot;

            childAnchor.DetachFromParent();
        }

        public Vector3 GetAbsolutePosition()
        {
            var modelPartMatrix = ModelPart.GetAbsoluteMatrix();
            return Vector3.TransformCoordinate(Position, modelPartMatrix);
        }

        public Anchor Clone(ModelPart parent)
        {
            return new Anchor(parent, Position, Name);
        }

        public void Rotate(Vector3 rotation)
        {
            Rotation += rotation;
        }

        private Quaternion GetRotation()
        {
            return Quaternion.RotationYawPitchRoll(Rotation.Y, -Rotation.X, Rotation.Z);
        }

        public Matrix GetRotationMatrix()
        {
            return Matrix.RotationYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
        }

        public Quaternion GetAbsoluteRotation()
        {
            return GetRotation() * ModelPart.GetAbsoluteRotation();
        }
    }
}