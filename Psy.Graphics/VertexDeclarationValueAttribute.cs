using System;

namespace Psy.Graphics
{
    public class VertexDeclarationValueAttribute : Attribute
    {
        public VertexDeclarationValueType VertexDeclarationValueType { get; set; }

        public VertexDeclarationValueAttribute(VertexDeclarationValueType vertexDeclarationValueType)
        {
            VertexDeclarationValueType = vertexDeclarationValueType;
        }
    }
}