using SlimMath;

namespace Psy.Core.Collision
{
    public class MeshBuilder<TMeshType> : IMeshBuilder<TMeshType> where TMeshType : Mesh
    {
        private byte _currentId;
        private readonly TMeshType _mesh;

        public MeshBuilder(TMeshType mesh)
        {
            _currentId = 0;
            _mesh = mesh;
        }

        public IMeshBuilder<TMeshType> WithId(byte id)
        {
            _currentId = id;
            return this;
        }

        public IMeshBuilder<TMeshType> Triangle(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            _mesh.AddTriangle(new MeshTriangle(_currentId, p0, p1, p2));
            return this;
        }

        public IMeshBuilder<TMeshType> Rectangle(Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight)
        {
            Triangle(bottomLeft, topLeft, topRight);
            Triangle(bottomLeft, topRight, bottomRight);
            return this;
        }

        public IMeshBuilder<TMeshType> Optimise()
        {
            _mesh.MergeTriangles();
            return this;
        }

        public TMeshType GetMesh()
        {
            return _mesh;
        }
    }
}