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

        public static void Write(this Stream stream, byte value)
        {
            stream.Write(new byte[]{value}, 0, 1);
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

        public static ushort ReadUShort(this Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static void Write(this Stream stream, short value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Write(this Stream stream, ushort value)
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

        public static ushort[] ReadUShortArray(this Stream stream)
        {
            var length = stream.ReadUShort();
            var ret = new ushort[length];
            for (var i = 0; i < length; ++i)
                ret[i] = (ushort) stream.ReadShort();
            return ret;
        }

        public static void Write(this Stream stream, ushort[] data)
        {
            stream.Write((ushort)data.Length);

            foreach (var t in data)
                stream.Write(t);
        }

        public static ushort[,] Read2DArray(this Stream stream)
        {
            var xMax = stream.ReadInt();
            var yMax = stream.ReadInt();

            var ret = new ushort[xMax, yMax];

            for (var x=0; x<xMax; ++x)
            {
                for (var y=0; y<yMax; ++y)
                {
                    ret[x, y] = stream.ReadUShort();
                }
            }

            return ret;
        }

        public static void Write(this Stream stream, ushort[,] data)
        {
            var xMax = data.GetLength(0);
            var yMax = data.GetLength(1);

            stream.Write(xMax);
            stream.Write(yMax);

            for (var x=0; x<xMax; ++x)
            {
                for (var y=0; y<yMax; ++y)
                {
                    stream.Write(data[x, y]);
                }
            }
        }

        public static ushort[][,] ReadUShortMap(this Stream stream)
        {
            var length = stream.ReadInt();
            var ret = new ushort[length][,];

            for (var i = 0; i < length; ++i)
            {
                ret[i] = stream.Read2DArray();
            }

            return ret;
        }

        public static void Write(this Stream stream, ushort[][,] data)
        {
            stream.Write(data.Length);
            for (var i=0; i<data.Length; ++i)
            {
                var array = data[i];
                stream.Write(array);
            }
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