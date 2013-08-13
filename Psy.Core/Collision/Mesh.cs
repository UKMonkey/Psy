using System;
using System.Linq;
using System.Collections.Generic;
using SlimMath;

namespace Psy.Core.Collision
{
    public class Mesh
    {
        public static Vector3 NoTranslation = new Vector3(0, 0, 0);
        protected List<MeshTriangle> Triangles { get; set; }

        public virtual int Id { get; set; }

        private Vector3 _translation;
        public virtual Vector3 Translation
            { get { return _translation; } set { _translation = value; } }

        public virtual float Rotation { get; set; }

        #region Max values
        private float? _maxX;
        private float? _maxY;
        private float? _maxZ;

        public float MaxX
        {
            get
            {
                if (_maxX == null)
                    _maxX = Triangles.SelectMany(triangle => triangle.Points).Max(point => point.X);
                return _maxX.Value;
            }
        }

        public float MaxY
        {
            get
            {
                if (_maxY == null)
                    _maxY = Triangles.SelectMany(triangle => triangle.Points).Max(point => point.Y);
                return _maxY.Value;
            }
        }

        public float MaxZ
        {
            get
            {
                if (_maxZ == null)
                    _maxZ = Triangles.SelectMany(triangle => triangle.Points).Max(point => point.Z);
                return _maxZ.Value;
            }
        }
#endregion

#region Min values
        private float? _minX;
        private float? _minY;
        private float? _minZ;


        public float MinX
        {
            get
            {
                if (_minX == null)
                    _minX = Triangles.SelectMany(triangle => triangle.Points).Min(point => point.X);
                return _minX.Value;
            }
        }

        public float MinY
        {
            get
            {
                if (_minY == null)
                    _minY = Triangles.SelectMany(triangle => triangle.Points).Min(point => point.Y);
                return _minY.Value;
            }
        }

        public float MinZ
        {
            get
            {
                if (_minZ == null)
                    _minZ = Triangles.SelectMany(triangle => triangle.Points).Min(point => point.Z);
                return _minZ.Value;
            }
        }
#endregion

        public Mesh()
        {
            _translation = NoTranslation;
            Triangles = new List<MeshTriangle>();
        }

        public virtual Mesh CopyMesh()
        {
            var ret = new Mesh();
            CopyDataFrom(ret);
            return ret;
        }

        public virtual void CopyDataFrom(Mesh target)
        {
            foreach (var meshTriangle in target.Triangles)
                AddTriangle(meshTriangle);

            Translation = target.Translation;
            Rotation = target.Rotation;
        }

        public virtual void AddTriangle(MeshTriangle meshTriangle)
        {
            Triangles.Add(new MeshTriangle(meshTriangle.Id, meshTriangle.P0, meshTriangle.P1, meshTriangle.P2));
            _maxX = _maxY = _maxZ = _minX = _minY = _minZ = null;
        }

        public virtual IEnumerable<MeshTriangle> GetAllTriangles(out int count)
        {
            count = Triangles.Count;

            if (Math.Abs(Rotation) > 0.01f)
                return Triangles.Select(item => item.Rotate(Rotation));

            return Triangles;
        }

        // examine all the triangles, and try to merge any that can be.
        // optimisation assumption - all the triangles that can be merged are already next to each other...
        public void MergeTriangles()
        {
            MergeTriangles(Triangles);
        }

        private void MergeTriangles(List<MeshTriangle> triangles)
        {
            var startCount = triangles.Count;
            //Logger.Write("Attempting to merge triangles in mesh - starting with " + startCount, LoggerLevel.Trace);

            var toExamine = new MeshTriangle[4];
            var newTriangles = new List<MeshTriangle>();
            var position = 0;

            for (; position < triangles.Count - 4; ++position)
            {
                for (var count = 0; count < 4; ++count)
                {
                    toExamine[count] = triangles[position + count];
                }

                MeshTriangle[] result;
                if (!AttemptMergeTriangles(toExamine, out result))
                {
                    newTriangles.Add(triangles[position]);
                    continue;
                }

                position += 3;
                newTriangles.AddRange(result);
            }

            for (; position < triangles.Count; ++position)
                newTriangles.Add(triangles[position]);

            triangles.Clear();
            triangles.AddRange(newTriangles);
            var endCount = triangles.Count;

            if (endCount != startCount)
                MergeTriangles(triangles);

            //Logger.Write("Completed merge triangles in mesh - finished with " + endCount, LoggerLevel.Trace);
        }


        // attempt to merge the (4) given triangles.  if they can be merged then the result is populated 
        private bool AttemptMergeTriangles(MeshTriangle[] toTryToMerge, out MeshTriangle[] result)
        {
            // for 4 triangles to merge - then between the 4 triangles there should be only 6 unique points
            result = null;
            var points = new HashSet<Vector3>();

            foreach (var triangle in toTryToMerge)
            {
                points.Add(triangle.P0);
                points.Add(triangle.P1);
                points.Add(triangle.P2);
            }

            if (points.Count != 6)
                return false;

            // we need 2 lots of 3 sets of points to be able to merge, so the 6 points are to be split
            // and the 2 lines can be merged into new triangles.
            List<Vector3> lineA;
            List<Vector3> lineB;
            if (SplitPoints(toTryToMerge, out lineA, out lineB))
            {
                result = new MeshTriangle[2];
                result[0] = new MeshTriangle(toTryToMerge[0].Id, lineA[0], lineA[2], lineB[2]);
                result[1] = new MeshTriangle(toTryToMerge[0].Id, lineB[0], lineB[2], lineA[2]);
                return true;
            }

            return false;
        }


        private bool SplitPoints(MeshTriangle[] triangles, out List<Vector3> lineA, out List<Vector3> lineB)
        {
            var gradients = new Vector3[4][];
            lineA = lineB = null;

            for (var i = 0; i < 4; ++i)
            {
                gradients[i] = new Vector3[3];
                for (var j = 0; j < 3; ++j)
                {
                    gradients[i][j] = (triangles[i].Points[j] - triangles[i].Points[(j + 1)%3]).NormalizeRet();
                }
            }

            var lines = new List<List<Vector3>>();

            // now find the gradients that are the same on different triangles & where
            // the start & end points are the same...
            for (var triangleAIndex = 0; triangleAIndex < 4; ++triangleAIndex)
            {
                for (var triangleBIndex = 0; triangleBIndex < 4; ++triangleBIndex)
                {
                    for (var lineAIndex = 0; lineAIndex < 3; ++lineAIndex)
                    {
                        for (var lineBIndex = 0; lineBIndex < 3; ++lineBIndex)
                        {
                            if (gradients[triangleAIndex][lineAIndex] == gradients[triangleBIndex][lineBIndex] &&
                                (triangles[triangleAIndex].Points[(lineAIndex + 1)%3] ==
                                 triangles[triangleBIndex].Points[lineBIndex]))
                            {
                                lines.Add(new List<Vector3>
                                              {
                                                  triangles[triangleAIndex].Points[lineAIndex],
                                                  triangles[triangleAIndex].Points[(lineAIndex + 1)%3],
                                                  triangles[triangleBIndex].Points[(lineBIndex + 1)%3]
                                              });
                            }
                        }
                    }
                }
            }

            if (lines.Count != 2)
                return false;

            lineA = lines[0];
            lineB = lines[1];

            return true;
        }
    }
}
