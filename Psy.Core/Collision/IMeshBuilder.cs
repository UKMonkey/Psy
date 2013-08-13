using SlimMath;

namespace Psy.Core.Collision
{
    public interface IMeshBuilder<out TMeshType> where TMeshType : Mesh
    {
        IMeshBuilder<TMeshType> WithId(byte id);
        IMeshBuilder<TMeshType> Triangle(Vector3 p0, Vector3 p1, Vector3 p2);
        IMeshBuilder<TMeshType> Rectangle(Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight);
        IMeshBuilder<TMeshType> Optimise();
        TMeshType GetMesh();
    }
}