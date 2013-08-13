using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Textures
{
    public class DepthStencilSurface : WrappedSurface
    {
        public DepthStencilSurface(Device device)
        {
            GetSurface(device);
        }

        protected override sealed void GetSurface(Device device)
        {
            Surface = device.DepthStencilSurface;
        }
    }
}