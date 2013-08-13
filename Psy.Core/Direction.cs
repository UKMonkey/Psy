using System;
using SlimMath;

namespace Psy.Core
{
    [Flags]
    public enum Direction : byte
    {
        None  = 0,
        North = 1,
        South = 2,
        East = 4,
        West = 8,
        Up = 16,
        Down = 32,

        // convenience directions
        NorthEast = 5,
        NorthWest = 9,
        SouthEast = 6,
        SouthWest = 10
    }

    public static class DirectionUtil
    {
        private const Direction NorthAndSouth = Direction.North | Direction.South;
        private const Direction EastAndWest = Direction.East | Direction.West;
        private const Direction UpAndDown = Direction.Up | Direction.Down;

        public static Direction GetOppositeDirection(Direction dir)
        {
            var ret = Direction.None;

            if (dir.HasFlag(Direction.North))
                ret = MergeDirections(ret, Direction.South);

            if (dir.HasFlag(Direction.South))
                ret = MergeDirections(ret, Direction.North);

            if (dir.HasFlag(Direction.East))
                ret = MergeDirections(ret, Direction.West);

            if (dir.HasFlag(Direction.West))
                ret = MergeDirections(ret, Direction.East);

            if (dir.HasFlag(Direction.Up))
                ret = MergeDirections(ret, Direction.Down);

            if (dir.HasFlag(Direction.Down))
                ret = MergeDirections(ret, Direction.Up);

            return ret;
        }

        public static Direction MergeDirections(Direction dirA, Direction dirB)
        {
            var dirC = dirA | dirB;

            if ((dirC & NorthAndSouth) == NorthAndSouth)
                dirC &= ~NorthAndSouth;

            if ((dirC & EastAndWest) == EastAndWest)
                dirC &= ~EastAndWest;

            if ((dirC & UpAndDown) == UpAndDown)
                dirC &= ~UpAndDown;

            return dirC;
        }

        public static float GetRotationFromDirection(Direction direction)
        {
            var rotation = 0.0f;

            switch (direction)
            {
                case Direction.North:
                    rotation = (float)Math.PI/2;
                    break;
                case Direction.NorthEast:
                    rotation = (float)Math.PI/4;
                    break;
                case Direction.East:
                    rotation = 0;
                    break;
                case Direction.SouthEast:
                    rotation = (float)(Math.PI + (Math.PI/2) + (Math.PI/4));
                    break;
                case Direction.South:
                    rotation = (float)(Math.PI + (Math.PI/2));
                    break;
                case Direction.SouthWest:
                    rotation = (float)(Math.PI + (Math.PI/4));
                    break;
                case Direction.West:
                    rotation = (float)Math.PI;
                    break;
                case Direction.NorthWest:
                    rotation = (float)((Math.PI/2) + (Math.PI/4));
                    break;
                case Direction.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }

            return rotation;
        }

        public static Vector3 CalculateVector(float rotation)
        {
            return new Vector3
            {
                Y = (float)Math.Sin(rotation),
                X = (float)Math.Cos(rotation)
            };
        }

        public static Vector3 CalculateVector(float rotation, float distance)
        {
            return new Vector3
            {
                Y = (float)Math.Sin(rotation) * distance,
                X = (float)Math.Cos(rotation) * distance
            };
        }
    }
}
