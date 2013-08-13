using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Psy.Core;
using Psy.Core.Logging;
using Psy.Core.Tasks;
using Psy.Graphics.DirectX.Effects;
using Psy.Graphics.DirectX.Text;
using Psy.Graphics.DirectX.Textures;
using Psy.Graphics.Effects;
using Psy.Graphics.Text;
using SlimDX.Direct3D9;
using SlimDX.Windows;
using SlimMath;

namespace Psy.Graphics.DirectX
{
    internal class DirectXGraphicsContext : GraphicsContext
    {
        private readonly WindowAttributes _windowAttributes;
        private readonly Device _device;
        private readonly Direct3D _direct3D;
        private readonly RenderForm _form;
        private PresentParameters _presentParameters;
        
        private bool _alphaBlending;
        private bool _contextReset;
        private bool _isFormClosed;
        private readonly FontCache _fontCache;
        private readonly EffectCache _effectCache;
        private readonly TextureCache _textureCache;
        private TextureAreaHolder _blankTexture;
        private readonly VertexDeclarationStorage _vertexDeclarationStorage;
        private readonly List<WrappedCubeTexture> _cubeTextures;
        private readonly List<WrappedTexture> _unmanagedTextures;
        private static List<WrappedSurface> _wrappedSurfaces;

        public override bool AlphaBlending 
        {
            get { return _alphaBlending; }
            set 
            {
                _alphaBlending = value;
                ToggleAlphaBlending(value);
            }
        }

        public override CompareFunc AlphaFunc 
        { 
            set
            {
                _device.SetRenderState(RenderState.AlphaFunc, CompareFuncMapper.Map(value));
            }
        }

        public override Color4 ClearColour { get; set; }

        public override Point CursorPosition
        {
            get { return _form.PointToClient(Cursor.Position); }
        }

        public override bool ZBufferEnabled
        { 
            get 
            { 
                var renderState = _device.GetRenderState(RenderState.ZEnable);
                return (ZBufferType) renderState == ZBufferType.UseZBuffer;
            } 
            set { _device.SetRenderState(RenderState.ZEnable, value ? ZBufferType.UseZBuffer : ZBufferType.DontUseZBuffer); } 
        }

        public override bool ZWriteEnabled
        {
            get
            {
                return _device.GetRenderState(RenderState.ZWriteEnable) > 0;
            }
            set
            {
                _device.SetRenderState(RenderState.ZWriteEnable, value);
            }

        }

        public override TextureFilter MipFilter
        {
            get
            {
                var filter = (SlimDX.Direct3D9.TextureFilter)_device.GetSamplerState(0, SamplerState.MipFilter);
                return TextureFilterMapper.MapFrom(filter);
            }
            set { _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilterMapper.MapFrom(value)); }
        }

        public override TextureFilter MinFilter
        { 
            get 
            { 
                var filter = (SlimDX.Direct3D9.TextureFilter)_device.GetSamplerState(0, SamplerState.MinFilter);
                return TextureFilterMapper.MapFrom(filter);
            } 
            set { _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilterMapper.MapFrom(value)); } 
        }

        public override TextureFilter MagFilter
        {
            get
            {
                var filter = (SlimDX.Direct3D9.TextureFilter)_device.GetSamplerState(0, SamplerState.MagFilter);
                return TextureFilterMapper.MapFrom(filter);
            }
            set { _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilterMapper.MapFrom(value)); }
        }

        public override CullMode CullMode
        {
            get
            {
                var cullMode = (Cull) _device.GetRenderState(RenderState.CullMode);
                return CullModeMapper.MapFrom(cullMode);
            }
            set 
            { 
                var cullMode = CullModeMapper.MapFrom(value);
                _device.SetRenderState(RenderState.CullMode, cullMode);
            }
        }

        public override IWindowSize WindowSize
        { 
            get
            {
                return new WindowSize(_device);
            }
        }

        public override Matrix Projection
        {
            get 
            { 
                var proj = _device.GetTransform(TransformState.Projection);
                return MatrixMapper.MapFrom(proj);
            }
            set { _device.SetTransform(TransformState.Projection, MatrixMapper.MapFrom(value)); }
        }

        public override Matrix View
        {
            get
            {
                var proj = _device.GetTransform(TransformState.View);
                return MatrixMapper.MapFrom(proj);
            }
            set { _device.SetTransform(TransformState.View, MatrixMapper.MapFrom(value)); }
        }

        public override Matrix World
        {
            get
            {
                var proj = _device.GetTransform(TransformState.World);
                return MatrixMapper.MapFrom(proj);
            }
            set { _device.SetTransform(TransformState.World, MatrixMapper.MapFrom(value)); }
        }

        public override FillMode FillMode
        {
            get 
            { 
                var fillMode = (SlimDX.Direct3D9.FillMode) _device.GetRenderState(RenderState.FillMode);
                return FillModeMapper.MapFrom(fillMode);
            }
            set { _device.SetRenderState(RenderState.FillMode, FillModeMapper.MapFrom(value)); }
        }

        public override TextureAreaHolder BlankTexture 
        { 
            get { return _blankTexture ?? (_blankTexture = GetTexture("noTexture.png")); }
        }

        public override ZCompareFunction ZCompareFunctionFunction
        {
            get
            {
                var compare = (Compare)_device.GetRenderState(RenderState.ZFunc);
                return ZCompareFunctionMapper.MapFrom(compare);
            }
            set { _device.SetRenderState(RenderState.ZFunc, ZCompareFunctionMapper.MapFrom(value)); }
        }

        public override ISurface RenderTarget
        {
            get
            {
                var wrappedSurface = WrappedSurface.GetRenderTarget(_device, 0);
                _wrappedSurfaces.Add(wrappedSurface);
                return wrappedSurface;
            }
            set
            {
                _device.SetRenderTarget(0, ((WrappedSurface) value).Surface);
            }
        }

        public override ISurface DepthStencilSurface
        {
            get
            {
                var wrappedSurface = WrappedSurface.DepthStencilSurface(_device);
                _wrappedSurfaces.Add(wrappedSurface);
                return wrappedSurface;
            }
            set
            {
                _device.DepthStencilSurface = ((WrappedSurface) value).Surface;
            }
        }

        public override bool SingleChannelColourWrite
        {
            set
            {
                _device.SetRenderState(RenderState.ColorWriteEnable, value ? ColorWriteEnable.Red : ColorWriteEnable.All);
            }
        }

        public override bool AlphaTest
        {
            get
            {
                return _device.GetRenderState<bool>(RenderState.AlphaTestEnable);
            }
            set
            {
                _device.SetRenderState(RenderState.AlphaTestEnable, value);
            }
        }

        public override int AlphaRef
        {
            get
            {
                return _device.GetRenderState<int>(RenderState.AlphaRef);
            }
            set
            {
                _device.SetRenderState(RenderState.AlphaRef, value);
            }
        }

        public DirectXGraphicsContext(WindowAttributes windowAttributes)
        {
            _windowAttributes = windowAttributes;

            _cubeTextures = new List<WrappedCubeTexture>();

            _form = new RenderForm(windowAttributes.Title)
            {
                ClientSize = new Size(
                    _windowAttributes.Width,
                    _windowAttributes.Height),
                FormBorderStyle = _windowAttributes.AllowResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle,
                MaximizeBox = _windowAttributes.AllowResize,
            };

            _unmanagedTextures = new List<WrappedTexture>();
            _wrappedSurfaces = new List<WrappedSurface>();

            _presentParameters = PresentParametersFactory.CreateFor(_form);

            _direct3D = new Direct3D();
            _device = new Device(
                _direct3D, 0, DeviceType.Hardware,
                _presentParameters.DeviceWindowHandle,
                CreateFlags.HardwareVertexProcessing,
                _presentParameters);

#if DEBUG
            SlimDX.Configuration.EnableObjectTracking = true;
#endif

            _vertexDeclarationStorage = new VertexDeclarationStorage();
            _effectCache = new EffectCache(_device);
            _textureCache = new TextureCache(_device);
            _textureCache.LoadRequiredTextures();

            StaticTaskQueue.TaskQueue.CreateRepeatingTaskWithDelay("Update texture cache", _textureCache.Update, 500, 100);

            _fontCache = new FontCache(_device);

            _form.Closed += (o, args) =>
            {
                _isFormClosed = true;
                _textureCache.Dispose();
            };
            _form.MouseMove += HandleFormMouseMove;
            _form.MouseDown += HandleFormMouseDown;
            _form.KeyDown += HandleFormKeyDown;
            _form.KeyUp += HandleFormKeyUp;
            _form.KeyPress += HandleFormKeyPress;
            _form.MouseUp += HandleFormMouseUp;
            _form.MouseWheel += HandleFormMouseWheel;

            _form.UserResized += (sender, args) =>
            {
                _contextReset = true;
                OnResize();
            };
        }

        private void HandleFormMouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        private void HandleFormMouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e);
        }

        private void HandleFormKeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        private void HandleFormKeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void HandleFormKeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void HandleFormMouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        private void HandleFormMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        private void ToggleAlphaBlending(bool enable)
        {
            _device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
            _device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);
            _device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
            _device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
            _device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
            _device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
            _device.SetRenderState(RenderState.AlphaBlendEnable, enable);
            _device.SetRenderState(RenderState.SourceBlend, Blend.One);
            _device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
        }

        public override void Clear(Color4 colour, float zDepth = 0.0f)
        {
            _device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, colour.ToColor4(), zDepth, 0);
        }

        public override void Clear(float zDepth = 0.0f)
        {
            _device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, ClearColour.ToColor4(), zDepth, 0);
        }

        public override void SwapBuffers()
        {
            _device.EndScene();
            _device.Present();
        }

        public override void Run()
        {
            MessagePump.Run(_form, () =>
            {
                if (_isFormClosed)
                    return;

                if (_contextReset)
                    ContextReset();

                StaticTaskQueue.TaskQueue.ProcessAll();

                DoRender();

                Thread.Yield();
            });
        }

        public override IFont GetFont(string fontFace = "", int fontSize = 16, Weight weight = Weight.Normal, bool italic = false)
        {
            return _fontCache.GetFont(fontFace, fontSize, weight, italic);
        }

        private void DoRender()
        {
            _device.SetRenderState(RenderState.Lighting, false);
            _device.BeginScene();

            Clear();

            OnRenderBegin();
            OnRender();

            SwapBuffers();
        }

        private void ContextReset()
        {
            try
            {
                _contextReset = false;

                _fontCache.DevicePreReset();
                _effectCache.DevicePreReset();
                UnmanagedTexturePreReset();

                _presentParameters = PresentParametersFactory.CreateFor(_form);

                _device.Reset(_presentParameters);

                _fontCache.DevicePostReset();
                _effectCache.DevicePostReset();
                UnmanagedTexturePostReset();
            }
            catch (SlimDX.SlimDXException)
            {
                Logger.Write("ContextReset failed", LoggerLevel.Critical);
                SlimDXUtils.DumpObjectTable();
                throw;
            }
        }

        private void UnmanagedTexturePostReset()
        {
            foreach (var wrappedCubeTexture in _cubeTextures)
            {
                wrappedCubeTexture.PostReset(_device);
            }

            foreach (var unmanagedTexture in _unmanagedTextures)
            {
                unmanagedTexture.PostReset(_device);
            }

            foreach (var wrappedSurface in _wrappedSurfaces)
            {
                wrappedSurface.PostReset(_device);
            }
        }

        private void UnmanagedTexturePreReset()
        {
            foreach (var wrappedCubeTexture in _cubeTextures)
            {
                wrappedCubeTexture.PreReset();
            }

            foreach (var unmanagedTexture in _unmanagedTextures)
            {
                unmanagedTexture.PreReset();
            }

            foreach (var wrappedSurface in _wrappedSurfaces)
            {
                wrappedSurface.PreReset();
            }
        }

        public override IVertexRenderer<T> CreateVertexRenderer<T>(int vertexCount)
        {
            return VertexRenderer<T>.Create(_vertexDeclarationStorage, vertexCount, _device);
        }

        public override IVertexRenderer<T> CreateIndexedVertexRenderer<T>(int vertexCount, int indexCount)
        {
            return VertexRenderer<T>.CreateIndexed(_vertexDeclarationStorage, vertexCount, indexCount, _device);
        }


        public override ICubeTexture CreateCubeTexture(int edgeSize, UsageType usageType, FormatType formatType)
        {
            var wrappedCubeTexture = WrappedCubeTexture.Create(_device, edgeSize, usageType, formatType);

            _cubeTextures.Add(wrappedCubeTexture);

            return wrappedCubeTexture;
        }

        public override ITexture CreateTexture(int width, int height, UsageType usageType, FormatType formatType)
        {
            var texture = WrappedTexture.Create(_device, width, height, usageType, formatType);
            _unmanagedTextures.Add(texture);
            
            return texture;
        }

        public override IList<TextureAreaHolder> LoadTextureAtlas(string textureAtlasFilename)
        {
            return _textureCache.LoadAtlas(textureAtlasFilename);
        }

        public override void LoadTextureAtlases(string atlasListFilename)
        {
            _textureCache.LoadAtlases(atlasListFilename);
        }

        public override TextureAreaHolder GetTexture(string textureName)
        {
            return _textureCache.GetByName(textureName);
        }

        public override IEffect CreateEffect(string filename)
        {
            return _effectCache.GetEffect(filename);
        }

        internal static Format MapFormat(FormatType formatType)
        {
            Format format;
            switch (formatType)
            {
                case FormatType.D16:
                    format = Format.D16;
                    break;
                case FormatType.R32F:
                    format = Format.R32F;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("formatType");
            }
            return format;
        }

        internal static Usage MapUsage(UsageType usageType)
        {
            Usage usage;
            switch (usageType)
            {
                case UsageType.DepthStencil:
                    usage = Usage.DepthStencil;
                    break;
                case UsageType.RenderTarget:
                    usage = Usage.RenderTarget;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("usageType");
            }
            return usage;
        }
    }

    
}