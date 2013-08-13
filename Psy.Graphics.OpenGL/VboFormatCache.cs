using System.Collections.Generic;

namespace Psy.Graphics.OpenGL
{
    public class VboFormatCache : VertexDeclarationStorageBase
    {
        public VboFormat GetFor<T>()
        {
            return new VboFormat();
        }
    }

    public class VboFormat
    {
        private List<VboFormatInstruction> _vboFormatInstructions;

        public void Apply()
        {
            
        }
    }

    internal class VboFormatInstruction
    {
        public VboFormatType VboFormatType;
        public short Offset;

    }

    internal enum VboFormatType
    {
        Vertex,
        Normal,
        Colour,
        SecondaryColour,
        Texture
    }
}