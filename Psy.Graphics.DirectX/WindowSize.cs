using Psy.Core;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public class WindowSize : IWindowSize
    {
        private readonly Device _device;

        public int Width
        {
            get { return _device.Viewport.Width; }
        }

        public int Height
        {
            get { return _device.Viewport.Height; }
        }

        public WindowSize(Device device)
        {
            _device = device;
        }
    }
}