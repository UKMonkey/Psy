using System.Collections.Generic;
using System.IO;
using SlimMath;

namespace Psy.Core.ThreedMesh.Reader.Wavefront
{
    public static class MaterialsReader
    {
        public static List<ModelMaterial> ReadFromStream(Stream stream)
        {
            var streamReader = new StreamReader(stream);

            var materialId = 0;
            var materials = new List<ModelMaterial>();
            ModelMaterial material = null;

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var parts = line.Split(' ');

                switch (parts[0])
                {
                    case "newmtl":
                        if (material != null)
                        {
                            materials.Add(material);
                        }
                        material = 
                            new ModelMaterial
                            {
                                MaterialId = materialId,
                                MaterialName = parts[1]
                            };
                        materialId++;
                        break;
                    case "map_Kd":
                        if (material == null)
                        {
                            throw new ModelReaderException("No material to load diffuse texture");
                        }
                        material.DiffuseTexture = parts[1];
                        break;
                    case "Ka":
                        if (material == null)
                        {
                            throw new ModelReaderException("No material to load ambient lighting");
                        }
                        material.Ambient = new Color4(
                            1.0f,
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3])
                            );
                        break;
                    case "Kd":
                        if (material == null)
                        {
                            throw new ModelReaderException("No material to load diffuse lighting");
                        }
                        material.Diffuse = new Color4(
                            1.0f,
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3])
                            );
                        break;
                    case "Ks":
                        if (material == null)
                        {
                            throw new ModelReaderException("No material to load specular lighting");
                        }
                        material.Specular = new Color4(
                            1.0f,
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3])
                            );
                        break;
                }
            }

            streamReader.Close();

            if (material != null)
            {
                materials.Add(material);
            }

            return materials;
        }
    }
}