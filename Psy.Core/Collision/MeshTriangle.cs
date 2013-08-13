using System.Collections.Generic;
using System.Diagnostics;
using SlimMath;

namespace Psy.Core.Collision
{
    public class MeshTriangle
    {
        public Direction Direction { get; private set; }
        public List<Vector3> Points { get; private set; } 

        public readonly Vector3 P0;
        public readonly Vector3 P1;
        public readonly Vector3 P2;

        // optimisations for the MeshTester
        public readonly Vector3 Normal;
        public readonly Vector3 U;
        public readonly Vector3 V;

        public readonly float UU;
        public readonly float VV;
        public readonly float UV;
        public readonly float D;
        
        public readonly byte Id;

        public MeshTriangle(byte id, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Id = id;
            P0 = p0;
            P1 = p1;
            P2 = p2;

            Points = new List<Vector3>{P0, P1, P2};
            U = P1 - P0;
            V = P2 - P0;
            Normal = U.Cross(V).NormalizeRet();

            Direction = CalculateDirection();
            UU = U.Dot(U);
            VV = V.Dot(V);
            UV = U.Dot(V);

            D = UV * UV - UU * VV;
        }

        private MeshTriangle(MeshTriangle other, Vector3 translate)
        {
            Id = other.Id;
            P0 = other.P0 + translate;
            P1 = other.P1 + translate;
            P2 = other.P2 + translate;

            Points = new List<Vector3>{P0, P1, P2};
            U = other.U;
            V = other.V;
            Normal = other.Normal;
            Direction = other.Direction;
            UU = other.UU;
            VV = other.VV;
            UV = other.UV;
            D = other.D;
        }

        public MeshTriangle Rotate(float angle)
        {
            return new MeshTriangle(Id, P0.Rotate(angle), P1.Rotate(angle), P2.Rotate(angle));
        }

        // for now only support North, South, East, West
        // may want to re-examine this later...
        private static readonly Vector3 EastVector  = new Vector3(1, 0, 0);
        private static readonly Vector3 SouthVector = new Vector3(0, -1, 0);
        private static readonly Vector3 UpVector    = new Vector3(0, 0, -1);

        private Direction CalculateDirection()
        {
            var eastDot = Normal.Dot(EastVector);
            var southDot = Normal.Dot(SouthVector);
            var upDot = Normal.Dot(UpVector);

            if (upDot < 0)
                return Direction.Up;
            if (upDot > 0)
                return Direction.Down;

            if (eastDot > 0 && southDot > 0)
                return Direction.SouthEast;
            if (eastDot < 0 && southDot > 0)
                return Direction.SouthWest;

            if (eastDot > 0 && southDot < 0)
                return Direction.NorthEast;
            if (eastDot < 0 && southDot < 0)
                return Direction.NorthWest;

            if (eastDot > 0)
                return Direction.East;
            if (eastDot < 0)
                return Direction.West;

            if (southDot > 0)
                return Direction.South;
            if (southDot < 0)
                return Direction.North;

            Debug.Assert(false);
            return Direction.None;
        }

        public MeshTriangle Translate(Vector3 amount)
        {
            return new MeshTriangle(this, amount);
        }

        public override string ToString()
        {
            return string.Format("{0}; {1}; {2}", P0, P1, P2);
        }
    }
}