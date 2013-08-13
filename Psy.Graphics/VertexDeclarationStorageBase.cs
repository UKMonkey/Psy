using System;
using System.Collections.Generic;
using SlimMath;

namespace Psy.Graphics
{
    public class VertexDeclarationStorageBase
    {
        protected enum FieldType
        {
            Vector2,
            Vector3,
            Vector4,
            Colour3,
            Colour4
        }

        protected class VertexDeclarationField
        {
            public readonly VertexDeclarationValueType VertexDeclarationValueType;
            public readonly FieldType FieldType;

            public VertexDeclarationField(VertexDeclarationValueType vertexDeclarationValueType, FieldType fieldType)
            {
                VertexDeclarationValueType = vertexDeclarationValueType;
                FieldType = fieldType;
            }
        }

        protected VertexDeclarationStorageBase() { }

        protected static IEnumerable<VertexDeclarationField> GetFields<T>()
        {
            var fields = typeof(T).GetFields();

            foreach (var fieldInfo in fields)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(VertexDeclarationValueAttribute), true);
                if (attrs.Length == 1)
                {
                    var attr = (VertexDeclarationValueAttribute)attrs[0];

                    if (fieldInfo.FieldType == typeof(Vector2))
                    {
                        yield return new VertexDeclarationField(attr.VertexDeclarationValueType, FieldType.Vector2);
                    }
                    else if (fieldInfo.FieldType == typeof(Vector3))
                    {
                        yield return new VertexDeclarationField(attr.VertexDeclarationValueType, FieldType.Vector3);
                    }
                    else if (fieldInfo.FieldType == typeof(Vector4))
                    {
                        yield return new VertexDeclarationField(attr.VertexDeclarationValueType, FieldType.Vector4);
                    }
                    else if (fieldInfo.FieldType == typeof(Color3))
                    {
                        yield return new VertexDeclarationField(attr.VertexDeclarationValueType, FieldType.Colour3);
                    }
                    else if (fieldInfo.FieldType == typeof(Color4))
                    {
                        yield return new VertexDeclarationField(attr.VertexDeclarationValueType, FieldType.Colour4);
                    }
                    else
                    {
                        throw new Exception(string.Format("Unknown vertex declaration field type `{0}`", fieldInfo.FieldType.Name));
                    }
                }
                    
            }
        }
    }
}