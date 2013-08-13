using System;
using System.Collections.Generic;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public class VertexDeclarationStorage : VertexDeclarationStorageBase
    {
        private readonly Dictionary<string, VertexDeclaration> _vertexDeclarations;

        public VertexDeclarationStorage()
        {
            _vertexDeclarations = new Dictionary<string, VertexDeclaration>(10);
        }

        internal VertexDeclaration GetFor<T>(Device device)
        {
            var typeName = typeof(T).Name;

            if (_vertexDeclarations.ContainsKey(typeName))
                return _vertexDeclarations[typeName];

            short offset = 0;
            var vertexElements = new List<VertexElement>(4);

            foreach (var fieldInfo in GetFields<T>())
            {
                var mappedUsage = MapUsageFrom(fieldInfo.VertexDeclarationValueType);

                switch (fieldInfo.FieldType)
                {
                    case FieldType.Vector2:
                        vertexElements.Add(new VertexElement(0, offset, DeclarationType.Float2, DeclarationMethod.Default, mappedUsage, 0));
                        offset += sizeof(float) * 2;
                        break;
                    case FieldType.Vector3:
                    case FieldType.Colour3:
                        vertexElements.Add(new VertexElement(0, offset, DeclarationType.Float3, DeclarationMethod.Default, mappedUsage, 0));
                        offset += sizeof(float) * 3;
                        break;
                    case FieldType.Colour4:
                    case FieldType.Vector4:
                        vertexElements.Add(new VertexElement(0, offset, DeclarationType.Float4, DeclarationMethod.Default, mappedUsage, 0));
                        offset += sizeof(float) * 4;
                        break;
                    default:
                        throw new Exception(string.Format("Unknown vertex declaration field type `{0}`", fieldInfo.FieldType));
                }
            }

            vertexElements.Add(VertexElement.VertexDeclarationEnd);
            var vertexDeclaration = new VertexDeclaration(device, vertexElements.ToArray());
            _vertexDeclarations[typeName] = vertexDeclaration;

            return vertexDeclaration;
        }

        private static DeclarationUsage MapUsageFrom(VertexDeclarationValueType vertexDeclarationValueType)
        {
            switch (vertexDeclarationValueType)
            {
                case VertexDeclarationValueType.Position:
                    return DeclarationUsage.Position;

                case VertexDeclarationValueType.PositionTransformed:
                    return DeclarationUsage.PositionTransformed;

                case VertexDeclarationValueType.TextureCoordinate:
                    return DeclarationUsage.TextureCoordinate;

                case VertexDeclarationValueType.Colour:
                    return DeclarationUsage.Color;

                case VertexDeclarationValueType.Normal:
                    return DeclarationUsage.Normal;

                default:
                    throw new ArgumentOutOfRangeException("vertexDeclarationValueType");
            }
        }
    }
}