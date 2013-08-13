using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Psy.Core;
using Psy.Graphics.Effects;
using Psy.Graphics.Text;
using SlimMath;

namespace Psy.Graphics
{
    public abstract class GraphicsContext
    {
        public abstract bool AlphaTest { get; set; }
        public abstract bool AlphaBlending { get; set; }
        public abstract int AlphaRef { get; set; }
        public abstract CompareFunc AlphaFunc { set; }
        public abstract Color4 ClearColour { get; set; }
        public abstract Point CursorPosition { get; }
        public abstract bool ZBufferEnabled { get; set; }
        public abstract bool ZWriteEnabled { get; set; }
        public abstract TextureFilter MipFilter { get; set; }
        public abstract TextureFilter MinFilter { get; set; }
        public abstract TextureFilter MagFilter { get; set; }
        public abstract CullMode CullMode { get; set; }
        public abstract IWindowSize WindowSize { get; }
        public abstract Matrix Projection { get; set; }
        public abstract Matrix View { get; set; }
        public abstract Matrix World { get; set; }
        public abstract FillMode FillMode { get; set; }
        public abstract TextureAreaHolder BlankTexture { get; }
        public abstract ZCompareFunction ZCompareFunctionFunction { get; set; }
        public abstract ISurface RenderTarget { get; set; }
        public abstract ISurface DepthStencilSurface { get; set; }
        public abstract bool SingleChannelColourWrite { set; }
        public abstract void Clear(Color4 colour, float zDepth = 0);
        public abstract void Clear(float zDepth = 0);
        public abstract void SwapBuffers();
        public abstract void Run();
        public abstract IFont GetFont(string fontFace = "", int fontSize = 16, Weight weight = Weight.Normal, bool italic = false);
        public abstract IVertexRenderer<T> CreateVertexRenderer<T>(int vertexCount) where T : struct;
        public abstract IVertexRenderer<T> CreateIndexedVertexRenderer<T>(int vertexCount, int indexCount) where T : struct;
        public abstract ICubeTexture CreateCubeTexture(int edgeSize, UsageType usageType, FormatType formatType);
        public abstract ITexture CreateTexture(int width, int height, UsageType usageType, FormatType formatType);
        public abstract IList<TextureAreaHolder> LoadTextureAtlas(string textureAtlasFilename);
        public abstract void LoadTextureAtlases(string atlasListFilename);
        public abstract TextureAreaHolder GetTexture(string textureName);
        public abstract IEffect CreateEffect(string filename);

        public event KeyPressEvent KeyPress;

        protected void OnKeyPress(KeyPressEventArgs args)
        {
            var handler = KeyPress;
            if (handler != null) handler(this, args);
        }

        public event MouseEvent MouseDown;

        protected void OnMouseDown(MouseEventArgs args)
        {
            var handler = MouseDown;
            if (handler != null) handler(this, args);
        }

        public event MouseEvent MouseUp;

        protected void OnMouseUp(MouseEventArgs args)
        {
            var handler = MouseUp;
            if (handler != null) handler(this, args);
        }

        public event MouseEvent MouseMove;

        protected void OnMouseMove(MouseEventArgs args)
        {
            var handler = MouseMove;
            if (handler != null) handler(this, args);
        }

        public event MouseEvent MouseWheel;

        protected void OnMouseWheel(MouseEventArgs args)
        {
            var handler = MouseWheel;
            if (handler != null) handler(this, args);
        }

        public event KeyEvent KeyDown;

        protected void OnKeyDown(KeyEventArgs args)
        {
            var handler = KeyDown;
            if (handler != null) handler(this, args);
        }

        public event KeyEvent KeyUp;

        protected void OnKeyUp(KeyEventArgs args)
        {
            var handler = KeyUp;
            if (handler != null) handler(this, args);
        }

        public event RenderEvent RenderBegin;

        protected void OnRenderBegin()
        {
            var handler = RenderBegin;
            if (handler != null) handler();
        }

        public event RenderEvent Render;

        protected void OnRender()
        {
            var handler = Render;
            if (handler != null) handler();
        }

        public event Action Resize;

        protected void OnResize()
        {
            var handler = Resize;
            if (handler != null) handler();
        }
    }
}