using SlimMath;

namespace Psy.Core.EpicModel
{
    public class RenderArgs
    {
        public class RenderArgsFace
        {
            public Color4 Colour;

            public RenderArgsFace(Color4 colour)
            {
                Colour = colour;
            }
        }

        public int MaterialId;
        public readonly RenderArgsFace[] RenderArgsFaces;

        public RenderArgs()
        {
            MaterialId = 0;
            RenderArgsFaces = new RenderArgsFace[6];
            for (var i = 0; i < 6; i++)
            {
                RenderArgsFaces[i] = new RenderArgsFace(Colours.White);
            }
        }

        public RenderArgsFace this[int face]
        {
            get { return RenderArgsFaces[face]; }
        }

        public static RenderArgs CreateDefault()
        {
            var result = new RenderArgs();
            return result;
        }

        public RenderArgs(ModelPart modelPart)
        {
            MaterialId = modelPart.MaterialId;
            RenderArgsFaces = new RenderArgsFace[modelPart.Faces.Length];
            for (var i = 0; i < modelPart.Faces.Length; i++)
            {
                RenderArgsFaces[i] = new RenderArgsFace(modelPart.Faces[i].Colour);
            }
        }
    }
}