using System;
using System.Collections.Generic;
using System.Linq;

namespace Psy.Core.Collision
{
    public class StaticMesh : Mesh
    {
        private float _actualRotation;
        public override float Rotation
        {
            get { return _actualRotation; }
            set
            {
                var difference = value - _actualRotation;
                if (Math.Abs(difference) < 0.01f)
                    return;
                _actualRotation = value;

                Triangles = Triangles.Select(item => item.Rotate(difference)).ToList();
            } 
        }

        public override Mesh CopyMesh()
        {
            var ret = new StaticMesh();
            CopyDataFrom(ret);
            return ret;
        }

        public override IEnumerable<MeshTriangle> GetAllTriangles(out int count)
        {
            count = Triangles.Count;
            return Triangles;
        }

        public override void AddTriangle(MeshTriangle meshTriangle)
        {
            Triangles.Add(new MeshTriangle(meshTriangle.Id, meshTriangle.P0, meshTriangle.P1, meshTriangle.P2).Rotate(_actualRotation));
        }
    }
}
