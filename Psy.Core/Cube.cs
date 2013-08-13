using System;
using System.Collections.Generic;
using SlimMath;

namespace Psy.Core
{
    public class Cube
    {
        public Vector3 TopFrontLeft { get { return _topFrontLeft.Value; } }
        private Vector3? _topFrontLeft;
        private Vector3? _topBackLeft;
        private Vector3? _topFrontRight;
        private Vector3? _topBackRight;

        public Vector3 BottomBackRight { get { return _bottomBackRight.Value; } }
        private Vector3? _bottomBackRight;
        private Vector3? _bottomFrontRight;
        private Vector3? _bottomBackLeft;
        private Vector3? _bottomFrontLeft;

        public Vector3 TopBackLeft
        {
            get
            { 
                if (!_topBackLeft.HasValue)
                    _topBackLeft = new Vector3(Left, Back, Top);
                return _topBackLeft.Value;
            }
        }

        public Vector3 TopFrontRight
        {
            get
            {
                if (!_topFrontRight.HasValue)
                    _topFrontRight = new Vector3(Right, Front, Top);
                return _topFrontRight.Value;
            }
        }

        public Vector3 TopBackRight
        {
            get
            {
                if (!_topBackRight.HasValue)
                    _topBackRight = new Vector3(Right, Back, Top);
                return _topBackRight.Value;
            }
        }

        public Vector3 BottomFrontRight
        {
            get
            {
                if (!_bottomFrontRight.HasValue)
                    _bottomFrontRight = new Vector3(Right, Front, Bottom);
                return _bottomFrontRight.Value;
            }
        }

        public Vector3 BottomBackLeft
        {
            get
            {
                if (!_bottomBackLeft.HasValue)
                    _bottomBackLeft = new Vector3(Left, Back, Bottom);
                return _bottomBackLeft.Value;
            }
        }

        public Vector3 BottomFrontLeft
        {
            get
            {
                if (!_bottomFrontLeft.HasValue)
                    _bottomFrontLeft = new Vector3(Left, Front, Bottom);
                return _bottomFrontLeft.Value;
            }
        }

        protected float Top { get { return TopFrontLeft.Z; } }
        protected float Bottom { get { return BottomBackRight.Z; } }

        protected float Front { get { return TopFrontLeft.Y; } }
        protected float Back { get { return BottomBackRight.Y; } }

        protected float Left { get { return TopFrontLeft.X; } }
        protected float Right { get { return BottomBackRight.X; } }

        public Cube(IEnumerable<Vector3> points)
        {
            var list = points;
            AddPoints(list);
        }


        private void AddPoints(IEnumerable<Vector3> points)
        {
            foreach (var point in points)
                AddPoint(point);
        }

        private void AddPoint(Vector3 point)
        {
            _topBackLeft = _topFrontRight = _topBackRight = 
                _bottomFrontRight = _bottomBackLeft = _bottomFrontLeft = null;

            if (_bottomBackRight == null || _topFrontLeft == null)
            {
                _bottomBackRight = point;
                _topFrontLeft = point;
            }

            if (_bottomBackRight.Value.X < point.X || 
                _bottomBackRight.Value.Y < point.Y ||
                _bottomBackRight.Value.Z < point.Z)
            {
                _bottomBackRight = new Vector3(
                    Math.Max(point.X, _bottomBackRight.Value.X),
                    Math.Max(point.Y, _bottomBackRight.Value.Y), 
                    Math.Max(point.Z, _bottomBackRight.Value.Z));
            }

            if (_topFrontLeft.Value.X > point.X ||
                _topFrontLeft.Value.Y > point.Y ||
                _topFrontLeft.Value.Z > point.Z)
            {
                _topFrontLeft = new Vector3(
                    Math.Min(point.X, _topFrontLeft.Value.X),
                    Math.Min(point.Y, _topFrontLeft.Value.Y),
                    Math.Min(point.Z, _topFrontLeft.Value.Z));
            }
        }
    }
}
