using SlimMath;

namespace Psy.Core
{
    public class EasedVector
    {
        private float _destX;
        private float _x;
        public float X
        {
            get { return _x; }
            set { _destX = value; }
        }

        private float _destY;
        private float _y;
        public float Y
        {
            get { return _y; }
            set { _destY = value; }
        }

        public void Update()
        {
            _x += ((_destX - _x) * 0.1f);
            _y += ((_destY - _y) * 0.1f);
        }

        public Vector3 AsVector()
        {
            return new Vector3(_x, _y, 0);
        }
    }
}