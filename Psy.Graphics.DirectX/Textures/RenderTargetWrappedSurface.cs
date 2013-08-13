using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Textures
{
    public class RenderTargetWrappedSurface : WrappedSurface
    {
        private readonly int _index;

        public RenderTargetWrappedSurface(Device device, int index)
        {
            _index = index;
            GetSurface(device);
        }

        protected override sealed void GetSurface(Device device)
        {
            Surface = device.GetRenderTarget(_index);
        }
    }
}