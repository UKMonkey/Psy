using System;
using System.IO;
using SlimMath;

namespace Psy.Core.Serialization
{
    public static class StreamExtensions
    {
        /************************************************************************/
        /* BASIC TYPES                                                          */
        /************************************************************************/
        public static float ReadFloat(this Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static long ReadLong(this Stream stream)
        {
            var buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static void Write(this Stream stream, long value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Write(this Stream stream, float value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static short ReadShort(this Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static void Write(this Stream stream, short value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static int ReadInt(this Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static void Write(this Stream stream, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static string ReadString(this Stream stream)
        {
            var length = ReadInt(stream);
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        public static void Write(this Stream stream, string value)
        {
            var ascii = new System.Text.UTF8Encoding();
            var bytes = ascii.GetBytes(value);
            Write(stream, bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static bool ReadBool(this Stream stream)
        {
            return stream.ReadByte() == 1;
        }

        public static void Write(this Stream stream, bool value)
        {
            stream.WriteByte((byte)(value ? 1 : 0));
        }

        /************************************************************************/
        /* CUSTOM TYPES                                                         */
        /************************************************************************/

        public static Color4 ReadColour(this Stream stream)
        {
            return new Color4(
                stream.ReadByte(),
                stream.ReadByte(),
                stream.ReadByte(),
                stream.ReadByte());
        }

        public static void Write(this Stream stream, Color4 colour)
        {
            var a = (byte)(255 * colour.Alpha);
            var r = (byte)(255 * colour.Red);
            var g = (byte)(255 * colour.Green);
            var b = (byte)(255 * colour.Blue);
            stream.WriteByte(a);
            stream.WriteByte(r);
            stream.WriteByte(g);
            stream.WriteByte(b);
        }

        public static Vector3 ReadVector(this Stream stream)
        {
            return new Vector3(stream.ReadFloat(), stream.ReadFloat(), stream.ReadFloat());
        }

        public static void Write(this Stream stream, Vector3 vector)
        {
            Write(stream, vector.X);
            Write(stream, vector.Y);
            Write(stream, vector.Z);
        }
    }
}