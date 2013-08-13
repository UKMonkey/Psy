using System.Collections.Generic;
using System.IO;
using SlimMath;

namespace Psy.Core
{
    public static class BinaryReaderWriterExtension
    {
        public static void Write(this BinaryWriter writer, Vector3 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        public static Vector3 ReadVector(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();

            return new Vector3(x, y, z);
        }

        public static void WriteVectorXY(this BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }

        public static Vector2 ReadVectorXY(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();

            return new Vector2(x, y);
        }

        public static void WriteVectorList(this BinaryWriter writer, List<Vector3> vectors)
        {
            writer.Write(vectors.Count);
            foreach (var vector in vectors)
            {
                writer.Write(vector);
            }
        }

        public static List<Vector3> ReadVectorList(this BinaryReader reader)
        {
            var list = new List<Vector3>();
            var count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadVector());
            }

            return list;
        }

        public static void WriteVectorListXY(this BinaryWriter writer, List<Vector2> vectors)
        {
            writer.Write(vectors.Count);
            foreach (var vector in vectors)
            {
                writer.WriteVectorXY(vector);
            }
        }

        public static List<Vector2> ReadVectorListXY(this BinaryReader reader)
        {
            var list = new List<Vector2>();
            var count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadVectorXY());
            }

            return list;
        }

        public static void WriteColour(this BinaryWriter writer, Color4 colour)
        {
            writer.Write(colour.Alpha);
            writer.Write(colour.Red);
            writer.Write(colour.Green);
            writer.Write(colour.Blue);
        }

        public static Color4 ReadColour(this BinaryReader reader)
        {
            var a = reader.ReadSingle();
            var r = reader.ReadSingle();
            var g = reader.ReadSingle();
            var b = reader.ReadSingle();
            return new Color4(a, r, g, b);
        }
    }
}