using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Psy.Core;
using Psy.Core.Console;
using Psy.Core.FileSystem;
using Rectangle = System.Drawing.Rectangle;

namespace Psy.Graphics.Textures
{
    public abstract class TextureCacheBase : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct RgbaColor
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        private class Task
        {
            public string FileName;
            public TextureAreaHolder ToUpdate;
        }

        protected readonly Dictionary<string, TextureAreaHolder> CacheByName;
        protected readonly Dictionary<int, TextureAreaHolder> CacheById;
        protected readonly Dictionary<string, List<TextureAreaHolder>> Atlases;
        protected int NextTextureId { get; private set; }

        private const bool UseThreadToLoadTextures = false;

        private const string BlankTexture = "noTexture.png";
        private bool _requiresUpdate = false;
        private bool _performWork = true;
        private readonly Thread _worker;
        private readonly List<Task> _tasks;

        protected TextureCacheBase()
        {
            CacheByName = new Dictionary<string, TextureAreaHolder>(20);
            CacheById = new Dictionary<int, TextureAreaHolder>(20);
            Atlases = new Dictionary<string, List<TextureAreaHolder>>(10);
            _tasks = new List<Task>();
            NextTextureId = 1;
            _requiresUpdate = false;

            _worker = new Thread(WorkerMain);

            if (UseThreadToLoadTextures)
                _worker.Start();
        }

        public void LoadRequiredTextures()
        {
            RegisterTexture(new TextureAreaHolder(GetTextureArea(BlankTexture)), BlankTexture);
            Update();
        }

        public void Update()
        {
            if (!_requiresUpdate)
                return;

            foreach (var item in CacheByName.Values)
                item.Update();
            foreach (var item in Atlases.SelectMany(item => item.Value))
                item.Update();

            _requiresUpdate = false;
        }

        private void WorkerMain()
        {
            while (_performWork)
            {
                var tasks = new List<Task>();
                lock(_tasks)
                {
                    if (_tasks.Count == 0)
                        Monitor.Wait(_tasks);
                    tasks.AddRange(_tasks);
                    _tasks.Clear();
                }

                foreach (var task in tasks)
                {
                    UpdateTextureArea(task.ToUpdate, task.FileName);
                }
            }
        }

        protected static unsafe void PrecalculateAlpha(Bitmap bitmap)
        {
            var bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            Debug.Assert(bitmapData.PixelFormat == PixelFormat.Format32bppArgb, "Invalid pixel format");
            Debug.Assert(bitmapData.Stride == bitmapData.Width * sizeof(RgbaColor), "Invalid pixel stride");

            var ptr = (byte*)bitmapData.Scan0.ToPointer();

            for (var y = 0; y < bitmapData.Height; y++)
            {
                for (var x = 0; x < bitmapData.Width; x++)
                {
                    // premultiply!
                    var color = (RgbaColor*)(ptr + (y * bitmapData.Stride) + (x * sizeof(RgbaColor)));

                    var alphaFloat = (*color).Alpha / 255.0f;

                    (*color).Red = Convert.ToByte(alphaFloat * (*color).Red);
                    (*color).Green = Convert.ToByte(alphaFloat * (*color).Green);
                    (*color).Blue = Convert.ToByte(alphaFloat * (*color).Blue);
                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        public TextureAreaHolder GetByName(string name)
        {
            if (CacheByName.ContainsKey(name))
            {
                return CacheByName[name];
            }

            var cachedTexture = new TextureAreaHolder(CacheByName[BlankTexture].TextureArea);
            RegisterTexture(cachedTexture, name);

            if (UseThreadToLoadTextures)
            {
                lock(_tasks)
                {
                    _tasks.Add(new Task{FileName = name, ToUpdate = cachedTexture});
                    Monitor.PulseAll(_tasks);
                }
            }
            else
            {
                UpdateTextureArea(cachedTexture, name);
                cachedTexture.Update();
            }

            return cachedTexture;
        }

        private void RegisterTexture(TextureAreaHolder cachedTexture, string name)
        {
            CacheById[NextTextureId] = cachedTexture;
            CacheByName[name] = cachedTexture;
            NextTextureId++;            
        }

        private TextureArea GetTextureArea(string name)
        {
            var filepath = Lookup.GetAssetPath(name);
            var bitmap = new Bitmap(filepath);
            var ret = CreateCachedTexture(name, bitmap);
            bitmap.Dispose();

            return ret;
        }

        private void UpdateTextureArea(TextureAreaHolder holder, string name)
        {
            holder.TextureArea = GetTextureArea(name);
            _requiresUpdate = true;
        }

        protected abstract TextureArea CreateCachedTexture(string name, Bitmap bitmap);


        public void LoadAtlases(string atlasListFile)
        {
            var assetPath = Lookup.GetAssetPath(atlasListFile);

            if (!File.Exists(assetPath))
            {
                return;
            }

            var filenames = File.ReadAllLines(assetPath);

            foreach (var filename in filenames)
            {
                LoadAtlas(filename);
            }
        }

        /// <summary>
        /// Load a texture atlas.
        /// </summary>
        /// <param name="filename">Name of the atlas definition file</param>
        /// <returns></returns>
        public List<TextureAreaHolder> LoadAtlas(string filename)
        {
            if (Atlases.ContainsKey(filename))
            {
                return Atlases[filename];
            }

            var textureFilename = "";
            var definitions = new List<TextureAtlasTextureDefinition>();

            using (var stream = new StreamReader(Lookup.GetAssetPath(filename)))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    if (line.StartsWith(";"))
                        continue;

                    var parts = line.Split('=');
                    var key = parts[0];
                    var value = parts[1];

                    switch (key)
                    {
                        case "filename":
                            textureFilename = value;
                            break;

                        case "texture":
                            var definition = TextureAtlasTextureDefinition.Parse(value);
                            definitions.Add(definition);
                            break;
                    }
                }
            }

            if (String.IsNullOrEmpty(textureFilename))
            {
                throw new ArgumentException(String.Format("No texture filename for atlas {0}", filename));
            }

            var textureAreas = UpdateTextureAreas(textureFilename, definitions);
            Atlases[filename] = textureAreas;
            return textureAreas;
        }
        
        private void UpdateAtlasSection(TextureAreaHolder target, TextureAreaHolder content, TextureAtlasTextureDefinition def)
        {
            target.TextureArea = CreateTextureArea(content.TextureArea, def);
        }

        private List<TextureAreaHolder> UpdateTextureAreas(string textureFilename, List<TextureAtlasTextureDefinition> definitions)
        {
            var cachedTexture = GetByName(textureFilename);
            var cachedTextureArea = cachedTexture.TextureArea;

            foreach (var definition in definitions)
            {
                definition.NormalizeTextureCoordinates(cachedTextureArea.Width, cachedTextureArea.Height);
                CacheByName[definition.Name] = new TextureAreaHolder(cachedTextureArea.Clone(definition));
                TextureAtlasTextureDefinition definition1 = definition;
                cachedTexture.OnChange += item => CacheByName[definition1.Name].TextureArea = item.TextureArea;
            }

            var textureAreas = new List<TextureAreaHolder>(definitions.Count);
            foreach (var definition in definitions)
            {
                var holder = new TextureAreaHolder(CreateTextureArea(cachedTextureArea, definition));
                var definition1 = definition;
                cachedTexture.OnChange += item => UpdateAtlasSection(holder, item, definition1);
            }

            return textureAreas;
        }

        protected abstract TextureArea CreateTextureArea(TextureArea cachedTexture, TextureAtlasTextureDefinition definition);

        public virtual void Dispose()
        {
            _performWork = false;
            lock(_tasks)
            {
                Monitor.PulseAll(_tasks);
            }
            if (UseThreadToLoadTextures)
                _worker.Join();
        }

        private static string GetFlagLegend()
        {
            return "L=Loaded, U=Unloaded, A=Atlas";
        }

        /// <summary>
        /// Write texture information to the console
        /// </summary>
        public void ConsoleDebugDump(string filter)
        {
            var console = StaticConsole.Console;
            console.AddLine(GetFlagLegend(), Colours.LightBlue);
            foreach (var cachedTexture in CacheByName)
            {
                if (cachedTexture.Key.Contains(filter))
                {
                    console.AddLine(cachedTexture.ToString(), Colours.LightBlue);    
                }
            }
        }
    }
}