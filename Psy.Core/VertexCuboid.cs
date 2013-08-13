using SlimMath;

namespace Psy.Core
{
    public class VertexCuboid
    {
        private Vector3 _size;

        private Vector3 Adjust { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Size
        {
            get { return _size; }
            set
            {
                Adjust = value / 2;
                _size = value;
            }
        }

        public Vector3 LeftFrontTop
        {
            get
            {
                return Position.Translate(-Adjust.X, -Adjust.Y, -Adjust.Z);
            }
        }

        public Vector3 RightFrontTop
        {
            get
            {
                return Position.Translate(Adjust.X, -Adjust.Y, -Adjust.Z);
            }
        }

        public Vector3 LeftFrontBottom
        {
            get
            {
                return Position.Translate(-Adjust.X, -Adjust.Y, Adjust.Z);
            }
        }

        public Vector3 RightFrontBottom
        {
            get
            {
                return Position.Translate(Adjust.X, -Adjust.Y, Adjust.Z);
            }
        }

        public Vector3 LeftBackTop
        {
            get
            {
                return Position.Translate(-Adjust.X, Adjust.Y, -Adjust.Z);
            }
        }

        public Vector3 RightBackTop
        {
            get
            {
                return Position.Translate(Adjust.X, Adjust.Y, -Adjust.Z);
            }
        }

        public Vector3 LeftBackBottom
        {
            get
            {
                return Position.Translate(-Adjust.X, Adjust.Y, Adjust.Z);
            }
        }

        public Vector3 RightBackBottom
        {
            get
            {
                return Position.Translate(Adjust.X, Adjust.Y, Adjust.Z);
            }
        }

        public VertexCuboid(Vector3 size)
        {
            Size = size;
        }
    }
}