using System;
using System.Collections.Generic;
using System.Linq;
using SlimMath;

namespace Psy.Core.ThreedMesh
{
    public class Model
    {
        public int Id;
        public readonly List<ModelObject> ModelObject;
        public List<ModelMaterial> Materials;
        public readonly List<Vector3> Vertices;
        public readonly List<Vector2> TextureCoordinates;

        private float? _topVertexZ;
        public float TopVertexZ
        {
            get
            {
                if (_topVertexZ == null)
                    _topVertexZ = Vertices.Min(v => v.Z);
                return _topVertexZ.Value;
            }
        }

        private float? _farY;
        public float FarY
        {
            get
            {
                if (_farY == null)
                    _farY = Vertices.Max(v => v.Y);
                return _farY.Value;
            }
        }

        private float? _radius;
        public float Radius
        {
            get
            {
                if (_radius == null)
                {
                    var maxZ = Vertices.Max(item => item.X * item.X + item.Y * item.Y);
                    _radius = (float) Math.Sqrt(maxZ);
                }
                return _radius.Value;
            }
        }

        public Model()
        {
            ModelObject = new List<ModelObject>();
            Materials = new List<ModelMaterial>();
            Vertices = new List<Vector3>();
            TextureCoordinates = new List<Vector2>();
        }
    }
}