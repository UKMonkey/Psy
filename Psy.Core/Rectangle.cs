using System;
using System.Diagnostics.Contracts;
using SlimMath;

namespace Psy.Core
{
    public struct Rectangle
    {
        public Vector2 TopLeft;
        public Vector2 BottomRight;

        public Vector2 BottomLeft { get { return new Vector2(TopLeft.X, BottomRight.Y); } }
        public Vector2 TopRight { get { return new Vector2(BottomRight.X, TopLeft.Y); } }

        public Vector2[] Points
        {
            get { return new[] {TopLeft, TopRight, BottomLeft, BottomRight}; }
        }

        public float Width
        {
            get { return BottomRight.X - TopLeft.X; }
        }

        public float Height
        {
            get { return BottomRight.Y - TopLeft.Y; }
        }

        /// <summary>
        /// Returns the length of the largest dimension.
        /// </summary>
        public float Size
        {
            get
            {
                return Math.Max(
                    (int)(BottomRight.Y - TopLeft.Y), 
                    (int)(BottomRight.X - TopLeft.X));
            }
        }

        public float Area
        {
            get { return Width*Height; }
        }

        private Vector2 _middle;
        public Vector2 Middle
        {
            get {return _middle;}
        }

        public static Rectangle FromBoundingBox(Vector2 bottomLeft, Vector2 topRight)
        {
            return new Rectangle
                       {
                           BottomRight = new Vector2(topRight.X, bottomLeft.Y),
                           TopLeft = new Vector2(bottomLeft.X, topRight.Y)
                       };
        }

        public static Rectangle Parse(string input)
        {
            var parts = input.Split(',');
            if (parts.Length == 4)
            {
                return new Rectangle
                              {
                                  TopLeft = new Vector2
                                  {
                                      X = int.Parse(parts[0]), 
                                      Y = int.Parse(parts[1])
                                  },
                                  BottomRight = new Vector2
                                  {
                                      X = int.Parse(parts[2]), 
                                      Y = int.Parse(parts[3])
                                  }
                              };            
            }

            throw new ArgumentException("Invalid rectangle string");
        }

        public Rectangle(Vector2 topLeft, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            _middle = new Vector2((TopLeft.X + bottomRight.X) / 2, 
                                    (BottomRight.Y + TopLeft.Y) / 2);
        }

        public bool Intersects(Rectangle other)
        {
            return
                !(BottomRight.X < other.TopLeft.X ||
                  TopLeft.X > other.BottomRight.X ||
                  BottomRight.Y > other.TopLeft.Y ||
                  TopLeft.Y < other.BottomRight.Y);
        }

        public Vector2 RandomPointInside()
        {
            var x = StaticRng.Random.Next((int) (TopLeft.X * 1000), (int) (BottomRight.X * 1000));
            var y = StaticRng.Random.Next((int) (BottomRight.Y * 1000), (int) (TopLeft.Y * 1000));

            return new Vector2(x / 1000.0f, y / 1000.0f);
        }

        [Pure]
        public bool Contains(Vector2 position)
        {
            // Yum.

            if (Height > 0)
            {
                // origin is top-left

                return
                    (
                        (position.X >= TopLeft.X) &&
                        (position.X < BottomRight.X) &&
                        (position.Y >= TopLeft.Y) &&
                        (position.Y < BottomRight.Y)
                    );
            }
            // origin is bottom-left
            return
                (
                    (position.X >= TopLeft.X) &&
                    (position.X < BottomRight.X) &&
                    (position.Y >= BottomRight.Y) &&
                    (position.Y < TopLeft.Y)
                );
        }

        public Rectangle Translate(float x, float y)
        {
            return new Rectangle
                       {
                           TopLeft = TopLeft + new Vector2(x, y),
                           BottomRight = BottomRight + new Vector2(x, y)
                       };
        }

        public Rectangle Translate(Vector2 vector)
        {
            return Translate(vector.X, vector.Y);
        }

        public static Rectangle CenteredAround(Vector3 position, Vector3 size)
        {
            var x = position.X - size.X/2.0f;
            var y = position.Y - size.Y/2.0f;
            return new Rectangle
                       {
                           TopLeft = new Vector2(x, y), 
                           BottomRight = new Vector2(x + size.X, y + size.Y)
                       };
        }

        public Rectangle? IntersectingArea(Rectangle area)
        {
            if (!Intersects(area))
                return null;

            var left = Math.Max(TopLeft.X, area.TopLeft.X);
            var right = Math.Min(BottomRight.X, area.BottomRight.X);

            var top = Math.Min(TopLeft.Y, area.TopLeft.Y);
            var bottom = Math.Max(BottomRight.Y, area.BottomRight.Y);

            return new Rectangle(new Vector2(left, top), new Vector2(right, bottom));
        }

        public Rectangle Scale(float horizontalCrop, float verticalCrop)
        {
            var w = Width*horizontalCrop;
            var h = Height*verticalCrop;
            return new Rectangle(TopLeft, new Vector2(TopLeft.X + w, TopLeft.Y + h));
        }
    }
}
