using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Psy.Core.FileSystem;

namespace Psy.Core
{
    public class MaterialCache
    {
        private readonly Dictionary<int, Material> _materials;

        public MaterialCache()
        {
            _materials = new Dictionary<int, Material>();
        }

        public bool HasMaterial(int materialId)
        {
            return _materials.ContainsKey(materialId);
        }

        public Material this[int materialIndex]
        {
            get { return _materials[materialIndex]; }
        }

        /// <summary>
        /// Retrieve a material by name, returns null if the material does not exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Material GetByName(string name)
        {
            foreach (var material in _materials)
            {
                if (material.Value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return material.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve a material by texture name, returns null if the material does not exist.
        /// </summary>
        /// <param name="textureName"></param>
        /// <returns></returns>
        public Material GetByTextureName(string textureName)
        {
            foreach (var material in _materials)
            {
                if (material.Value.TextureName.Equals(textureName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return material.Value;
                }
            }
            return null;
        }

        public void LoadMaterials(string materialDefinitionFile)
        {
            _materials.Clear();

            var document = new XmlDocument();
            document.Load(Lookup.GetAssetPath(materialDefinitionFile));

            var rootNode = document.DocumentElement;

            if (rootNode == null)
            {
                throw new Exception("Failed to read materials");
            }

            foreach (var material in from XmlNode node in rootNode.ChildNodes select ParseItem(node)) {
                _materials[material.Id] = material;
            }
        }

        private static Material ParseItem(XmlNode node)
        {
            var id = int.Parse(GetNamedItem(node, "id").Value);
            var name = GetNamedItem(node, "name").Value;
            var textureName = GetNamedItem(node, "texture").Value;
            var walksound = GetNamedItem(node, "walkSound").Value;
            var outside = bool.Parse(GetNamedItem(node, "outside").Value);

            return new Material(id, name, textureName, walksound, outside);
        }

        private static XmlNode GetNamedItem(XmlNode item, string name)
        {
            return item.Attributes != null ?
                item.Attributes.GetNamedItem(name) : null;
        }

        public Material Add(string filepath)
        {
            var nextMaterialId = 0;
            if (_materials.Count > 0)
            {
                nextMaterialId = _materials.Max(x => x.Key) + 1;
            }

            var material = new Material(nextMaterialId, Path.GetFileNameWithoutExtension(filepath), filepath, "", false);

            _materials[nextMaterialId] = material;

            return material;
        }
    }
}