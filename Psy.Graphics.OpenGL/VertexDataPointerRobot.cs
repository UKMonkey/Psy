using System;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using SlimMath;

namespace Psy.Graphics.OpenGL
{
    internal static class VertexDataPointerRobot
    {
        public static bool PreTransformed { get; set; } // todo: shitty public static

        internal static void SetGLState<T>()
        {
            var fields = typeof(T).GetFields();

            var offset = 0;

            foreach (var fieldInfo in fields)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(VertexDeclarationValueAttribute), true);
                if (attrs.Length != 1)
                    continue;

                var attr = (VertexDeclarationValueAttribute)attrs[0];

                offset = ProcessVertexDeclarationValueType<T>(attr, fieldInfo, offset);
            }
        }

        private static int ProcessVertexDeclarationValueType<T>(
            VertexDeclarationValueAttribute attr, 
            FieldInfo fieldInfo,
            int offset)
        {
            //GL.DisableClientState(ArrayCap.VertexArray);
            //GL.DisableClientState(ArrayCap.ColorArray);
            //GL.DisableClientState(ArrayCap.NormalArray);
            //GL.DisableClientState(ArrayCap.TextureCoordArray);

            switch (attr.VertexDeclarationValueType)
            {
                case VertexDeclarationValueType.Colour:

                    GL.EnableClientState(ArrayCap.ColorArray); GLH.AssertGLError();

                    if (fieldInfo.FieldType == typeof (Color3))
                    {
                        GL.ColorPointer(3, ColorPointerType.Float, Marshal.SizeOf(typeof(T)), offset); GLH.AssertGLError();
                        offset += 3 * sizeof (float);
                    }
                    else if (fieldInfo.FieldType == typeof (Color4))
                    {
                        GL.ColorPointer(4, ColorPointerType.Float, Marshal.SizeOf(typeof(T)), offset); GLH.AssertGLError();
                        offset += 4 * sizeof (float);
                    }
                    else
                    {
                        throw new Exception(string.Format(
                            "Cannot use field type `{0}` for Colour vertex declaration",
                            fieldInfo.FieldType.Name));
                    }
                    break;

                case VertexDeclarationValueType.Normal:

                    GL.EnableClientState(ArrayCap.NormalArray); GLH.AssertGLError();

                    if (fieldInfo.FieldType == typeof (Vector3))
                    {
                        GL.NormalPointer(NormalPointerType.Float, Marshal.SizeOf(typeof(T)), offset); GLH.AssertGLError();
                        offset += 3 * sizeof (float);
                    }
                    else
                    {
                        throw new Exception(string.Format(
                            "Cannot use field type `{0}` for Normal vertex declaration",
                            fieldInfo.FieldType.Name));
                    }
                    break;

                case VertexDeclarationValueType.PositionTransformed:
                case VertexDeclarationValueType.Position:

                    PreTransformed = (attr.VertexDeclarationValueType == VertexDeclarationValueType.PositionTransformed);

                    GL.EnableClientState(ArrayCap.VertexArray); GLH.AssertGLError();

                    if (fieldInfo.FieldType == typeof (Vector3))
                    {
                        GL.VertexPointer(3, VertexPointerType.Float, Marshal.SizeOf(typeof(T)), offset); GLH.AssertGLError();
                        offset += 3 * sizeof (float);
                    }
                    else if (fieldInfo.FieldType == typeof (Vector4))
                    {
                        GL.VertexPointer(4, VertexPointerType.Float, Marshal.SizeOf(typeof(T)), offset); GLH.AssertGLError();
                        offset += 4 * sizeof (float);
                    }
                    else
                    {
                        throw new Exception(string.Format(
                            "Cannot use field type `{0}` for Position vertex declaration",
                            fieldInfo.FieldType.Name));
                    }
                    break;

                case VertexDeclarationValueType.TextureCoordinate:

                    GL.EnableClientState(ArrayCap.TextureCoordArray); GLH.AssertGLError();

                    if (fieldInfo.FieldType == typeof (Vector2))
                    {
                        GL.TexCoordPointer(2, TexCoordPointerType.Float, Marshal.SizeOf(typeof(T)), offset); GLH.AssertGLError();
                        offset += 2 * sizeof (float);
                    }
                    else
                    {
                        throw new Exception(string.Format(
                            "Cannot use field type `{0}` for Normal vertex declaration",
                            fieldInfo.FieldType.Name));
                    }
                    break;

                default:
                    throw new Exception(string.Format(
                        "Unknown vertex declaration field type `{0}`", 
                        fieldInfo.FieldType.Name));
            }
            return offset;
        }
    }
}