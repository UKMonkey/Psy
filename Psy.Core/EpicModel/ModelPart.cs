using System.Collections.Generic;
using System.Linq;
using SlimMath;

namespace Psy.Core.EpicModel
{
    public class ModelPart
    {
        private const float SelectAnchorRadius = 0.1f;
        private static int _nextId = 1;

        public readonly int Id;
        public int MaterialId;

        public string Name;
        protected int? SelectedFaceIndex;
        protected bool Selected;
        public Vector3 Rotation;
        public ModelPartFace[] Faces;
        public readonly List<Anchor> Anchors;
        public Vector3[] Vertices;

        public ModelPartFace SelectedFace
        {
            get
            {
                return SelectedFaceIndex.HasValue ? Faces[SelectedFaceIndex.Value] : null;
            }
        }

        public Vector3 Position { get; set; }

        public Vector3 Size
        {
            get { return GetSize(); }
            set { ScaleVertices(value.DivideBy(GetSize())); }
        }

        public Anchor Pivot
        {
            get { return Anchors[0]; }
        }

        public bool IsARootModelPart
        {
            get { return !Pivot.HasParent; }
        }

        private void ScaleVertices(Vector3 vector3)
        {
            for (var index = 0; index < Vertices.Length; index++)
            {
                Vertices[index] = Vertices[index].Scale(vector3);
            }
        }

        private Vector3 GetSize()
        {
            var maxX = Vertices.Max(m => m.X);
            var maxY = Vertices.Max(m => m.Y);
            var maxZ = Vertices.Max(m => m.Z);

            var minX = Vertices.Min(m => m.X);
            var minY = Vertices.Min(m => m.Y);
            var minZ = Vertices.Min(m => m.Z);

            return new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
        }

        public ModelPart()
        {
            Id = _nextId++;
            Anchors = new List<Anchor> { new Anchor(this, new Vector3(0.0f, 0.0f, 0.0f), "PIVOT") };
            Name = "";
        }

        public ModelPart(int id)
        {
            Anchors = new List<Anchor> { new Anchor(this, new Vector3(0.0f, 0.0f, 0.0f), "PIVOT") };
            Name = "";
            Id = id;
        }

        public ModelPart Clone()
        {
            var clone = new ModelPart
            {
                Name = Name + "_clone", 
                Vertices = new Vector3[Vertices.Length],
                MaterialId = MaterialId,
                Rotation = Rotation,
            };

            for (var i = 0; i < Vertices.Length; i++)
            {
                clone.Vertices[i] = Vertices[i];
            }

            clone.Faces = new ModelPartFace[Faces.Length];
            for (var i = 0; i < Faces.Length; i++)
            {
                clone.Faces[i] = Faces[i].Clone(clone);
            }

            clone.Pivot.Position = Pivot.Position;

            clone.Anchors.Capacity = Anchors.Count;
            foreach (var anchor in Anchors.Where(x => !x.IsPivot))
            {
                clone.Anchors.Add(anchor.Clone(this));
            }

            return clone;
        }

        public void SelectFace(int face)
        {
            SelectedFaceIndex = face;
        }

        public void Select()
        {
            Selected = true;
        }

        public void ClearSelect()
        {
            SelectedFaceIndex = null;
            Selected = false;
        }

        private Matrix GetRotationMatrix()
        {
            var quat = GetRotation();
            var rotationMatrix = Matrix.RotationQuaternion(quat);
            return rotationMatrix;
        }

        private Quaternion GetRotation()
        {
            return Quaternion.RotationYawPitchRoll(Rotation.Y, -Rotation.X, Rotation.Z);
        }

        private Matrix GetAnchorMatrix()
        {
            return Matrix.Translation(-Anchors[0].Position);
        }

        public PickModelResult IntersectsWithRay(Ray cameraRay)
        {
            var ray3 = GetModelRay(cameraRay);

            int? selectedFace = null;

            var minDistance = 1000f;

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

            if (selectedFace == null)
            {
                return PickModelResult.Nothing;
            }

            return new PickModelResult
            {
                DistanceFromCamera = minDistance,
                ModelPart = this,
                FaceIndex = (int)selectedFace
            };
        }

        public PickAnchorResult GetIntersectingAnchor(Ray cameraRay)
        {
            var ray3 = GetModelRay(cameraRay);

            foreach (var anchor in Anchors)
            {
                var sphere = new BoundingSphere(anchor.Position, SelectAnchorRadius);
                if (sphere.Intersects(ref ray3))
                {
                    return new PickAnchorResult { Anchor = anchor };
                }
            }

            return PickAnchorResult.Nothing;
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

        public Matrix GetAbsoluteMatrix()
        {
            var matrix = GetAnchorMatrix() * GetRotationMatrix();

            if (IsARootModelPart)
            {
                return matrix * Matrix.Translation(Position);
            }

            return matrix * Matrix.Translation(Pivot.Parent.Position) * Matrix.Translation(Position) * Pivot.Parent.ModelPart.GetAbsoluteMatrix();
        }

        public Vector3 GetAbsolutePosition()
        {
            var matrix = GetAbsoluteMatrix();
            return Vector3.TransformCoordinate(new Vector3(), matrix);
        }

        public Matrix GetAnchorRotationAndPositionMatrix()
        {
            var matrix = GetAnchorMatrix() * GetRotationMatrix();

            if (IsARootModelPart)
            {
                matrix = matrix * Matrix.Translation(Position);
            }

            return matrix;
        }

        public void SetRenderArgsSelection(RenderArgs renderArgs)
        {
            var selectedFaceColor = new Color4(1.0f, 1.0f, 0.2f, 0.2f);

            if (Selected)
            {
                foreach (var face in renderArgs.RenderArgsFaces)
                {
                    face.Colour = selectedFaceColor;
                }
            }
            else
            {
                if (SelectedFaceIndex != null)
                {
                    renderArgs[SelectedFaceIndex.Value].Colour = selectedFaceColor;
                }
            }
        }

        private IEnumerable<ModelTriangle> GetTriangles()
        {
            foreach (var modelPartFace in Faces)
            {
                foreach (var modelTriangle in modelPartFace.Triangles)
                {
                    yield return modelTriangle;
                }
            }
        }

        public Anchor AddAnchor(string name = "new_anchor")
        {
            var anchor = new Anchor(this, new Vector3(), name);
            Anchors.Add(anchor);
            return anchor;
        }

        public void RemoveAnchor(Anchor focusAnchor)
        {
            Position = Vector3.TransformCoordinate(new Vector3(), GetAbsoluteMatrix());
            Anchors.Remove(focusAnchor);
        }

        public BoundingBox GetBoundingBox()
        {
            return BoundingBox.FromPoints(Vertices);
        }

        public Quaternion GetAbsoluteRotation()
        {
            if (IsARootModelPart)
            {
                return GetRotation();
            }

            return GetRotation() * Pivot.Parent.ModelPart.GetAbsoluteRotation();
        }
    }
}