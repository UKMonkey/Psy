using SlimMath;

namespace Psy.Core
{
    public class TextureAtlasTextureDefinition
    {
        public readonly int Id;
        public readonly string Name;
        public Vector2 TopLeft { get; private set; }
        public Vector2 BottomRight { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private TextureAtlasTextureDefinition(int id, string name, Vector2 topLeft, Vector2 bottomRight, int width, int height)
        {
            Id = id;
            Name = name;
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Width = width;
            Height = height;
        }

        public static TextureAtlasTextureDefinition Parse(string definition)
        {
            var parts = definition.Split(':');
            var id = int.Parse(parts[0]);
            var name = parts[1];
            var rectangle = Rectangle.Parse(parts[2]);

            return new TextureAtlasTextureDefinition(
                id, name, rectangle.TopLeft, rectangle.BottomRight, (int)rectangle.Width, (int)rectangle.Height);
        }

        /// <summary>
        /// Normalize the pixels coordinates to the range of 0.0-1.0
        /// by specifying the dimensions of a texture
        /// </summary>
        /// <param name="width">Width of texture in pixels</param>
        /// <param name="height">Height of texture in pixels</param>
        public void NormalizeTextureCoordinates(int width, int height)
        {
            TopLeft = new Vector2
                          {
                              X = TopLeft.X / width,
                              Y = TopLeft.Y / height
                          };

            BottomRight = new Vector2
                              {
                                  X = BottomRight.X / width,
                                  Y = BottomRight.Y / height
                              };
        }
    }
}