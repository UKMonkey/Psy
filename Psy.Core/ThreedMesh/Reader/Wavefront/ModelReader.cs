using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Psy.Core.FileSystem;
using SlimMath;

namespace Psy.Core.ThreedMesh.Reader.Wavefront
{
    public class ModelReader : IModelReader
    {
        private bool ScaleModel { get; set; }
        private float ScaleTo { get; set; }

        public ModelReader()
        {
            ScaleModel = false;
            ScaleTo = 0.75f;
        }

        public Model ReadFromStream(Stream stream)
        {
            var model = new Model();
            var modelObject = new ModelObject();

            const int vertexIndex = 1;

            var streamReader = new StreamReader(stream);

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "mtllib":
                    {
                        // load material
                        var filename = parts[1];
                        model.Materials = ReadMaterials(filename);
                    }
                        break;
                    case "v":
                        model.Vertices.Add(
                            new Vector3(
                                -float.Parse(parts[1]),
                                -float.Parse(parts[3]),
                                -float.Parse(parts[2])
                                ));
                        break;
                    case "vt":
                        model.TextureCoordinates.Add(
                            new Vector2(
                                float.Parse(parts[1]),
                                float.Parse(parts[2])
                                ));
                        break;
                    case "f":
                        if (parts.Length == 5)
                        {
                            // face definition
                            var f1 = int.Parse(parts[1].Split('/')[0]);
                            var f2 = int.Parse(parts[2].Split('/')[0]);
                            var f3 = int.Parse(parts[3].Split('/')[0]);
                            var f4 = int.Parse(parts[4].Split('/')[0]);

                            var vt1 = int.Parse(parts[1].Split('/')[1]);
                            var vt2 = int.Parse(parts[2].Split('/')[1]);
                            var vt3 = int.Parse(parts[3].Split('/')[1]);
                            var vt4 = int.Parse(parts[4].Split('/')[1]);

                            var p0 = model.Vertices[f1-1];
                            var p1 = model.Vertices[f2-1];
                            var p2 = model.Vertices[f4-1];
                            var cross = ((p1 - p0).Cross(p2 - p0));
                            cross.Normalize();

                            modelObject.Faces.Add(
                                new Face
                                {
                                    Vertex = new Triangle
                                             {
                                                 PositionIndex0 = f1 - vertexIndex,
                                                 PositionIndex1 = f4 - vertexIndex,
                                                 PositionIndex2 = f2 - vertexIndex
                                             },
                                    FaceNormal = cross,
                                    TextureCoordinate = new Triangle
                                                        {
                                                            PositionIndex0 = vt1 - vertexIndex,
                                                            PositionIndex1 = vt4 - vertexIndex,
                                                            PositionIndex2 = vt2 - vertexIndex
                                                        }
                                });

                            modelObject.Faces.Add(
                                new Face
                                {
                                    Vertex = new Triangle
                                             {
                                                 PositionIndex0 = f2 - vertexIndex,
                                                 PositionIndex1 = f4 - vertexIndex,
                                                 PositionIndex2 = f3 - vertexIndex
                                             },
                                    FaceNormal = cross,
                                    TextureCoordinate = new Triangle
                                                        {
                                                            PositionIndex0 = vt2 - vertexIndex,
                                                            PositionIndex1 = vt4 - vertexIndex,
                                                            PositionIndex2 = vt3 - vertexIndex
                                                        }
                                });
                        }
                        else
                        {
                            // face definition
                            var f1 = int.Parse(parts[1].Split('/')[0]);
                            var f2 = int.Parse(parts[2].Split('/')[0]);
                            var f3 = int.Parse(parts[3].Split('/')[0]);

                            var p0 = model.Vertices[f1 - vertexIndex];
                            var p1 = model.Vertices[f3 - vertexIndex];
                            var p2 = model.Vertices[f2 - vertexIndex];
                            var cross = ((p1 - p0).Cross(p2 - p0));
                            cross.Normalize();

                            var vt1 = int.Parse(parts[1].Split('/')[1]);
                            var vt2 = int.Parse(parts[2].Split('/')[1]);
                            var vt3 = int.Parse(parts[3].Split('/')[1]);

                            modelObject.Faces.Add(
                                new Face
                                {
                                    Vertex = new Triangle
                                             {
                                                 PositionIndex0 = f1 - vertexIndex,
                                                 PositionIndex1 = f3 - vertexIndex,
                                                 PositionIndex2 = f2 - vertexIndex
                                             },
                                    FaceNormal = cross,
                                    TextureCoordinate = new Triangle
                                                        {
                                                            PositionIndex0 = vt1 - vertexIndex,
                                                            PositionIndex1 = vt3 - vertexIndex,
                                                            PositionIndex2 = vt2 - vertexIndex
                                                        }
                                });
                        }
                        break;
                    case "usemtl":
                    {
                        // material use definition
                        var material = model.Materials.FirstOrDefault(m => m.MaterialName == parts[1]);

                        if (material == null)
                        {
                            throw new ModelReaderException(
                                string.Format("No such material `{0}`", parts[1]));
                        }

                        if (modelObject.Faces.Count > 0)
                        {
                            model.ModelObject.Add(modelObject);
                            modelObject = new ModelObject();
                        }

                        modelObject.MaterialIndex = material.MaterialId;

                    }
                        break;
                }
            }

            streamReader.Close();

            if (modelObject != null)
            {
                model.ModelObject.Add(modelObject);
            }

            // scale vertices in each into range

            if (ScaleModel)
            {

                var maxX = model.Vertices.Max(v => v.X);
                var maxY = model.Vertices.Max(v => v.Y);
                var maxZ = model.Vertices.Max(v => v.Z);

                var minX = model.Vertices.Min(v => v.X);
                var minY = model.Vertices.Min(v => v.Y);
                var minZ = model.Vertices.Min(v => v.Z);

                var xrange = maxX - minX;
                var yrange = maxY - minY;
                var zrange = maxZ - minZ;

                var range = Math.Max(Math.Max(xrange, yrange), zrange);

                for (var index = 0; index < model.Vertices.Count; index++)
                {
                    var vector = model.Vertices[index];
                    model.Vertices[index] =
                        new Vector3(
                            vector.X*(ScaleTo/range),
                            vector.Y * (ScaleTo / range),
                            ((vector.Z - maxZ) * (ScaleTo / range)));
                }
            }

            return model;
        }

        

        private static List<ModelMaterial> ReadMaterials(string filename)
        {
            List<ModelMaterial> materials;

            var path = Lookup.GetAssetPath(filename);

            using (var fileStream = 
                new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                materials = MaterialsReader.ReadFromStream(fileStream);
            }

            var baseName = Path.GetFileNameWithoutExtension(path);
            var xmtlName = baseName + ".xmtl";

            var xmtlPath = "";

            if (Lookup.AssetExists(xmtlName, out xmtlPath))
            {
                List<MaterialEx> xmtls;

                using (var fileStream =
                    new FileStream(xmtlPath, FileMode.Open, FileAccess.Read))
                {
                    xmtls = MaterialExReader.ReadFromStream(fileStream);
                }

                // match the material with its MaterialEx
                foreach (var modelMaterial in materials)
                {
                    var materialEx = xmtls
                        .SingleOrDefault(x => x.MaterialName == modelMaterial.MaterialName);

                    if (materialEx != null)
                    {
                        modelMaterial.MaterialEx = materialEx;
                    }
                }
            }

            return materials;
        }

        public static Model ReadFromFile(string filename)
        {
            var reader = new ModelReader();

            Model model;
            using (var stream =
                new FileStream(
                    filename,
                    FileMode.Open,
                    FileAccess.Read))
            {
                model = reader.ReadFromStream(stream);
            }

            return model;
        }
    }
}