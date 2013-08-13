using SlimDX.Direct3D9;
using SlimDX.Windows;

namespace Psy.Graphics.DirectX
{
    public static class PresentParametersFactory
    {
        public static PresentParameters CreateFor(RenderForm renderForm)
        {
            return new PresentParameters
                   {
                       BackBufferFormat = Format.A8R8G8B8,
                       BackBufferCount = 1,
                       BackBufferWidth = renderForm.ClientSize.Width,
                       BackBufferHeight = renderForm.ClientSize.Height,
                       Multisample = MultisampleType.None,
                       SwapEffect = SwapEffect.Flip,
                       EnableAutoDepthStencil = true,
                       AutoDepthStencilFormat = Format.D24X8,
                       PresentationInterval = PresentInterval.Default,
                       Windowed = true,
                       DeviceWindowHandle = renderForm.Handle
                   };
        }
    }
}