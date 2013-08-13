using System.Collections.Generic;
using System.IO;
using Psy.Core;
using Psy.Core.EpicModel.Serialization;
using Psy.Core.FileSystem;
using Psy.Graphics.Models.Compilers;

namespace Psy.Graphics.Models
{
    public class CompiledModelCache
    {
        private readonly IMaterialTranslator _materialTranslator;
        private readonly MaterialCache _materialCache;
        private readonly Dictionary<string, CompiledModel> _cachedCompiledModels; 

        public CompiledModelCache(
            IMaterialTranslator materialTranslator, 
            MaterialCache materialCache)
        {
            _materialTranslator = materialTranslator;
            _materialCache = materialCache;
            _cachedCompiledModels = new Dictionary<string, CompiledModel>(10);
        }

        public ModelInstance GetModel(string filename)
        {
            CompiledModel existingCompiledModel;
            if (_cachedCompiledModels.TryGetValue(filename, out existingCompiledModel))
            {
                return new ModelInstance(existingCompiledModel, _materialCache);
            }

            var model = ReadEpicModel(filename, Lookup.GetAssetPath(filename));
            var epicModelCompiler = new EpicModelCompiler(model);

            var compiledModel = epicModelCompiler.Compile();

            _cachedCompiledModels[filename] = compiledModel;

            return new ModelInstance(compiledModel, _materialCache);
        }

        private Core.EpicModel.EpicModel ReadEpicModel(string filename, string filepath)
        {
            using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(filestream))
                {
                    var epicModelReader = new EpicModelReader(_materialTranslator, reader);
                    var epicModel = epicModelReader.Read(filename);
                    return epicModel;
                }
            }
        }
    }
}