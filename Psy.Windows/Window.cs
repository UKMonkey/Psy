using System;
using System.Drawing;
using System.Windows.Forms;
using Psy.Core;
using Psy.Core.Logging;
using Psy.Graphics;

namespace Psy.Windows
{
    public delegate void DeviceResetEvent();
    public delegate void FormClosedEvent();

    public class Window : IDisposable
    {
        public static event DeviceResetEvent DevicePreReset;
        public static event DeviceResetEvent DevicePostReset;
        public event FormClosedEvent FormClosed;
        protected DebugKeyHandler DebugKeyHandler { get; private set; }
        private readonly WindowAttributes _windowAttributes;
        protected readonly KeyboardStatus KeyboardStatus;
        public GraphicsContext GraphicsContext;

        public string Title
        {
            get { return _windowAttributes.Title; }
            set { _windowAttributes.Title = value; }
        }

        public Point MousePosition { get { return GraphicsContext.CursorPosition; } }

        protected Window(WindowAttributes windowAttributes)
        {
            _windowAttributes = windowAttributes;
            KeyboardStatus = new KeyboardStatus();
            DebugKeyHandler = new DebugKeyHandler();
        }

        public void OnFormClosed()
        {
            var handler = FormClosed;
            if (handler != null) handler();
        }

        protected virtual void OnInitialize() {}
        protected virtual void OnResourceLoad() {}
        protected virtual void OnResourceUnload() {}

        protected virtual void OnRenderBegin() {}

        protected void AlphaBlending(bool enable)
        {
            GraphicsContext.AlphaBlending = enable;
        }

        protected virtual void OnRender() {}

        protected virtual void OnMouseMove(object sender, MouseEventArgs args) {}

        protected virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardStatus.SetPressedFlag(e.KeyCode, true);
        }

        protected virtual void OnKeyPress(object sender, KeyPressEventArgs e) { }

        protected virtual void OnMouseDown(object sender, MouseEventArgs e) { }

        protected virtual void OnMouseUp(object sender, MouseEventArgs e) { }

        protected virtual void OnKeyUp(object sender, KeyEventArgs e)
        {
            KeyboardStatus.SetPressedFlag(e.KeyCode, false);
        }

        protected virtual void OnMouseWheel(object sender, MouseEventArgs e) { }

        protected void Quit()
        {
            // todo: fix
            //_form.Close();
        }

        private void HandleDebugKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Control && DebugKeyHandler.HandleKey(args.KeyCode))
            {
                return;
            }

            OnKeyDown(sender, args);
        }

        public void Run()
        {
            var renderer = (Platform.GetExecutingPlatform() == PlatformType.Windows)
                               ? Renderer.DirectX
                               : Renderer.OpenGL;

            if (Environment.CommandLine.Contains("-opengl"))
            {
                renderer = Renderer.OpenGL;
            }

            GraphicsContext = GraphicsContextLoader.Create(renderer, _windowAttributes);

            GraphicsContext.MouseMove += OnMouseMove;
            GraphicsContext.MouseDown += OnMouseDown;
            GraphicsContext.KeyDown += HandleDebugKeyDown;
            GraphicsContext.KeyUp += OnKeyUp;
            GraphicsContext.KeyPress += OnKeyPress;
            GraphicsContext.MouseUp += OnMouseUp;
            GraphicsContext.MouseWheel += OnMouseWheel;
            GraphicsContext.RenderBegin += OnRenderBegin;
            GraphicsContext.Render += OnRender;
            GraphicsContext.Resize += OnResize;

            // todo: form closed?
            /*
            _form.Closed += (o, args) => { 
                isFormClosed = true;
                OnFormClosed();
            
            };
             */

            OnInitialize();
            OnResourceLoad();

            GraphicsContext.Run();

            OnResourceUnload();
        }

        private void OnResize()
        {
            // todo: fix this.
            try
            {
                //_contextReset = false;

                if (DevicePreReset != null)
                    DevicePreReset();

                if (DevicePostReset != null)
                    DevicePostReset();

            }
            catch (Exception e)
            {
                Logger.Write(String.Format("ContextReset failed ({0})", e), LoggerLevel.Critical);
                // todo: renable somehow
                //SlimDXUtils.DumpObjectTable();
                throw;
            }
        }

        public virtual void Dispose()
        {
            // todo: fix
            //_form.Dispose();
        }
    }
}
