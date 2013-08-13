using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Psy.Core;
using Psy.Core.FileSystem;
using Psy.Core.Logging;
using Psy.Core.Tasks;
using Psy.Graphics.Effects;
using Psy.Graphics.OpenGL.Text;
using Psy.Graphics.Text;
using SlimMath;
using Color4 = SlimMath.Color4;
using KeyPressEventArgs = OpenTK.KeyPressEventArgs;

namespace Psy.Graphics.OpenGL
{
    internal class OpenGLGraphicsContext : GraphicsContext
    {
        private const string VertexShaderFileExtension = ".v";
        private const string FragmentShaderFileExtension = ".f";

        private void ToggleBooleanCap(EnableCap enableCap, bool value)
        {
            if (value)
            {
                GL.Enable(enableCap);
            }
            else
            {
                GL.Disable(enableCap);
            }
        }

        public override bool AlphaTest
        {
            get
            {
                bool val;
                GL.GetBoolean(GetPName.AlphaTest, out val);
                return val;
            }
            set
            {
                ToggleBooleanCap(EnableCap.AlphaTest, value);    
            }
        }

        public override bool AlphaBlending
        {
            get 
            { 
                bool val;
                GL.GetBoolean(GetPName.Blend, out val);
                return val;
            }
            set
            {
                ToggleBooleanCap(EnableCap.Blend, value);
            }
        }

        public override int AlphaRef
        {
            get
            { 
                float alphaRef;
                GL.GetFloat(GetPName.AlphaTestRef, out alphaRef);
                return (int)(alphaRef * 255.0f);
            }
            set
            {
                int func;
                GL.GetInteger(GetPName.AlphaTestFunc, out func);
                GL.AlphaFunc((AlphaFunction)func, value / 255.0f);
            }
        }

        public override CompareFunc AlphaFunc
        {
            set
            {
                float alphaRef;
                GL.GetFloat(GetPName.AlphaTestRef, out alphaRef);
                GL.AlphaFunc(CompareFuncMapper.Map(value), alphaRef);
            }
        }

        private Color4 _clearColour;
        public override Color4 ClearColour
        {
            get { return _clearColour; } 
            set
            {
                _clearColour = value;
                GL.ClearColor(_clearColour.ToGLColor4());
            }
        }
        public override Point CursorPosition
        {
            get { return new Point(_gameWindow.Mouse.X, _gameWindow.Mouse.Y); }
        }

        public override bool ZBufferEnabled
        {
            get
            {
                GL.Enable(EnableCap.DepthTest);
                bool enabled;
                GL.GetBoolean(GetPName.DepthTest, out enabled);
                return enabled;
            }
            set
            {
                ToggleBooleanCap(EnableCap.DepthTest, value);
            }
        }
        public override bool ZWriteEnabled
        {
            get
            {
                bool enabled;
                GL.GetBoolean(GetPName.DepthWritemask, out enabled);
                return enabled;
            }
            set
            {
                GL.DepthMask(value);
            }
        }
        public override TextureFilter MipFilter
        {
            get
            {
                return TextureFilter.None;
            }
            set
            {
                // unused?
            }
        }


        public override TextureFilter MinFilter
        {
            get
            {
                int filter;
                GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureMinFilter, out filter);
                return MapTextureFilter(filter);
            }
            set
            {
                var filter = MapMinTextureFilter(value);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filter);
            }
        }

        public override TextureFilter MagFilter
        {
            get
            {
                int filter;
                GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureMagFilter, out filter);
                return MapTextureFilter(filter);
            }
            set
            {
                var filter = MapMaxTextureFilter(value);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filter);
            }
        }

        private TextureFilter MapTextureFilter(int filter)
        {
            switch (filter)
            {
                case (int)All.Linear:
                    return TextureFilter.Linear;

                case (int)All.Nearest:
                    return TextureFilter.Point;

                case (int)All.LinearMipmapNearest:
                    return TextureFilter.Anisotropic;

                default:
                    throw new ArgumentOutOfRangeException("filter");
            }
        }

        private int MapMaxTextureFilter(TextureFilter value)
        {
            switch (value)
            {
                case TextureFilter.None:
                case TextureFilter.Linear:
                    return (int) All.Linear;
                case TextureFilter.Point:
                    return (int) All.Nearest;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        private int MapMinTextureFilter(TextureFilter value)
        {
            switch (value)
            {
                case TextureFilter.None:
                    return (int) All.Nearest;

                case TextureFilter.Linear:
                    return (int) All.Linear;

                case TextureFilter.Anisotropic:
                    // todo: actually use anisotropic filtering
                    return (int) All.LinearMipmapNearest;

                case TextureFilter.Point:
                    return (int) All.Nearest;

                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public override CullMode CullMode
        {
            get
            {
                return CullMode.None;
            }
            set
            {
                if (value == CullMode.None)
                {
                    GL.Disable(EnableCap.CullFace);
                }
                else
                {
                    GL.Enable(EnableCap.CullFace);
                }

                if (value == CullMode.CCW)
                {
                    GL.CullFace(CullFaceMode.Front);
                }
                else
                {
                    GL.CullFace(CullFaceMode.Back);
                }
            }
        }

        public override IWindowSize WindowSize
        {
            get { return new WindowSize(_gameWindow); }
        }

        public override Matrix Projection
        {
            get
            {
                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 matrix;
                GL.GetFloat(GetPName.ProjectionMatrix, out matrix);
                return Matrix4ToMatrix(matrix);
            }
            set
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(value.ToArray());
            }
        }

        public override Matrix View
        {
            get { return _viewMatrix; }
            set
            {
                _viewMatrix = value;
                SetModelViewMatrix();
            }
        }

        public override Matrix World
        {
            get { return _worldMatrix; }
            set
            {
                _worldMatrix = value;
                SetModelViewMatrix();
            }
        }

        private void SetModelViewMatrix()
        {
            var mat = _worldMatrix * _viewMatrix;
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(mat.ToArray());
        }

        public override FillMode FillMode { get; set; } // todo

        public override TextureAreaHolder BlankTexture
        {
            get { return _blankTexture ?? (_blankTexture = GetTexture("noTexture.png")); }
        }

        public override ZCompareFunction ZCompareFunctionFunction { get; set; } // todo
        public override ISurface RenderTarget { get; set; } // todo
        public override ISurface DepthStencilSurface { get; set; } // todo
        public override bool SingleChannelColourWrite // todo
        {
            set { throw new NotImplementedException(); }
        }

        private readonly GameWindow _gameWindow;
        private readonly FontCache _fontCache;
        private readonly TextureCache _textureCache;
        private Matrix _worldMatrix;
        private Matrix _viewMatrix;
        private TextureAreaHolder _blankTexture;
        
        public OpenGLGraphicsContext(WindowAttributes windowAttributes)
        {
            _gameWindow = new GameWindow(
                windowAttributes.Width, windowAttributes.Height,
                GraphicsMode.Default, "Window", GameWindowFlags.Default, DisplayDevice.Default,
                2, 1, GraphicsContextFlags.Debug)
            {
                Title = windowAttributes.Title,
                WindowBorder = windowAttributes.AllowResize ? WindowBorder.Resizable : WindowBorder.Fixed,
            };
            _gameWindow.Context.ErrorChecking = true;

            Logger.Write(GL.GetString(StringName.Version), LoggerLevel.Info);
            Logger.Write(GL.GetString(StringName.ShadingLanguageVersion), LoggerLevel.Info);
            Logger.Write(GL.GetString(StringName.Renderer), LoggerLevel.Info);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front); GLH.AssertGLError();
            GL.DepthFunc(DepthFunction.Less); GLH.AssertGLError();

            GL.Disable(EnableCap.CullFace);

            _viewMatrix = Matrix.Identity;
            _worldMatrix = Matrix.Identity;

            _gameWindow.RenderFrame += GameWindowOnRenderFrame;
            _gameWindow.UpdateFrame += GameWindowOnUpdateFrame;
            _gameWindow.KeyDown += GameWindowOnKeyDown;
            _gameWindow.KeyUp += GameWindowOnKeyUp;
            _gameWindow.KeyPress += GameWindowOnKeyPress;
            _gameWindow.Mouse.ButtonDown += MouseOnButtonDown;
            _gameWindow.Mouse.ButtonUp += MouseOnButtonUp;
            _gameWindow.Mouse.Move += MouseOnMove;
            _gameWindow.Resize += GameWindowOnResize;

            _fontCache = new FontCache();
            _textureCache = new TextureCache();
            _textureCache.LoadRequiredTextures();
        }

        private void GameWindowOnResize(object sender, EventArgs eventArgs)
        {
            GL.Viewport(_gameWindow.ClientRectangle);
            QuickFont.QFont.InvalidateViewport();
            OnResize();
        }

        private void MouseOnMove(object sender, MouseMoveEventArgs mouseMoveEventArgs)
        {
            var e = new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, mouseMoveEventArgs.X, mouseMoveEventArgs.Y, 0);
            OnMouseMove(e);
        }

        private void GameWindowOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            OnKeyPress(new System.Windows.Forms.KeyPressEventArgs(keyPressEventArgs.KeyChar));
        }

        private void GameWindowOnKeyUp(object sender, KeyboardKeyEventArgs keyboardKeyEventArgs)
        {
            OnKeyUp(new KeyEventArgs(KeyMapper.MapFrom(keyboardKeyEventArgs.Key)));
        }

        private void GameWindowOnKeyDown(object sender, KeyboardKeyEventArgs keyboardKeyEventArgs)
        {
            OnKeyDown(new KeyEventArgs(KeyMapper.MapFrom(keyboardKeyEventArgs.Key)));
        }

        private void MouseOnButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var mouseButtons = MouseButtons.None;
            
            switch (mouseButtonEventArgs.Button)
            {
                case MouseButton.Left:
                    mouseButtons = MouseButtons.Left;
                    break;
                case MouseButton.Right:
                    mouseButtons = MouseButtons.Right;
                    break;
                case MouseButton.Middle:
                    mouseButtons = MouseButtons.Middle;
                    break;
            }

            OnMouseUp(new System.Windows.Forms.MouseEventArgs(
                mouseButtons, 1, mouseButtonEventArgs.X, mouseButtonEventArgs.Y, 0));
        }

        private void MouseOnButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var mouseButtons = MouseButtons.None;

            switch (mouseButtonEventArgs.Button)
            {
                case MouseButton.Left:
                    mouseButtons = MouseButtons.Left;
                    break;
                case MouseButton.Right:
                    mouseButtons = MouseButtons.Right;
                    break;
                case MouseButton.Middle:
                    mouseButtons = MouseButtons.Middle;
                    break;
            }
            OnMouseDown(new System.Windows.Forms.MouseEventArgs(
                mouseButtons, 1, mouseButtonEventArgs.X, mouseButtonEventArgs.Y, 0));
        }

        private void GameWindowOnUpdateFrame(object sender, FrameEventArgs frameEventArgs)
        {
            StaticTaskQueue.TaskQueue.ProcessAll();
            Thread.Yield();
        }

        private void GameWindowOnRenderFrame(object sender, FrameEventArgs frameEventArgs)
        {
            Clear();
            OnRender();
            SwapBuffers();
        }

        public override void Clear(Color4 colour, float zDepth = 0)
        {
            GL.ClearColor(colour.ToGLColor4());
            Clear();
        }

        public override void Clear(float zDepth = 0)
        {
            Clear();
        }

        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearDepth(1.0);
            GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
        }

        public override void SwapBuffers()
        {
            _gameWindow.SwapBuffers();
        }

        public override void Run()
        {
            _gameWindow.Run();
        }

        public override IFont GetFont(string fontFace = "", int fontSize = 16, Weight weight = Weight.Normal, bool italic = false)
        {
            return _fontCache.GetFont(fontFace == "" ? "Arial" : fontFace, fontSize, weight, italic);
        }

        public override IVertexRenderer<T> CreateVertexRenderer<T>(int vertexCount)
        {
            return new VertexRenderer<T>(this, vertexCount);
        }

        public override IVertexRenderer<T> CreateIndexedVertexRenderer<T>(int vertexCount, int indexCount)
        {
            throw new NotImplementedException();
            return new VertexRenderer<T>(this, vertexCount);
        }

        public override ICubeTexture CreateCubeTexture(int edgeSize, UsageType usageType, FormatType formatType)
        {
            throw new NotImplementedException();
            return new CubeTexture();
        }

        public override ITexture CreateTexture(int width, int height, UsageType usageType, FormatType formatType)
        {
            // todo: complete
            return new Texture();
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
            var withoutExtension = Path.GetFileNameWithoutExtension(filename);

            var vertexShaderFilename = withoutExtension + VertexShaderFileExtension;
            var fragmentShaderFilename = withoutExtension + FragmentShaderFileExtension;

            if (!Lookup.AssetExists(vertexShaderFilename))
            {
                throw new Exception(string.Format("Can't find vertex shader with filename {0}", vertexShaderFilename));
            }
            if (!Lookup.AssetExists(fragmentShaderFilename))
            {
                throw new Exception(string.Format("Can't find fragment shader with filename {0}", fragmentShaderFilename));
            }

            var vertexShaderText = File.ReadAllText(Lookup.GetAssetPath(vertexShaderFilename));
            var fragmentShaderText = File.ReadAllText(Lookup.GetAssetPath(fragmentShaderFilename));

            var effect = Effect.Create(this, vertexShaderText, fragmentShaderText);

            return effect;
        }

        private static Matrix Matrix4ToMatrix(Matrix4 matrix)
        {
            return new Matrix(
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
                );
        }

        private void DebugCallback(int id, ArbDebugOutput category, ArbDebugOutput severity,
            IntPtr length, string message, IntPtr userparam)
        {
            Console.WriteLine(message);
        }
    }
}