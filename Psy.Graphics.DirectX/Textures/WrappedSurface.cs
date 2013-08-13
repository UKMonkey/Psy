using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Textures
{
    public abstract class WrappedSurface : ISurface
    {
        internal Surface Surface;

        public string DebugName { get; set; }

        public static WrappedSurface CubeMapSurface(CubeMapFaceEnum cubeMapFaceEnum,
                                                       WrappedCubeTexture wrappedCubeTexture)
        {
            return new CubeMapSurface(cubeMapFaceEnum, wrappedCubeTexture);
        }

        public static WrappedSurface GetRenderTarget(Device device, int index)
        {
            return new RenderTargetWrappedSurface(device, index);
        }

        public static WrappedSurface DepthStencilSurface(Device device)
        {
            return new DepthStencilSurface(device);
        }

        public static WrappedSurface TextureSurface(Texture texture, int surfaceLevel)
        {
            return new TextureSurface(texture, surfaceLevel);
        }

        public void Dispose()
        {
            Surface.Dispose();
        }

        public void PreReset()
        {
            Surface.Dispose();
        }

        protected abstract void GetSurface(Device device);

        public void PostReset(Device device)
        {
            GetSurface(device);
        }
    }

    public class TextureSurface : WrappedSurface
    {
        private readonly Texture _texture;
        private readonly int _surfaceLevel;

        public TextureSurface(Texture texture, int surfaceLevel)
        {
            _texture = texture;
            _surfaceLevel = surfaceLevel;
        }

        protected override void GetSurface(Device device)
        {
            Surface = _texture.GetSurfaceLevel(_surfaceLevel);
        }
    }

    public class CubeMapSurface : WrappedSurface
    {
        private readonly CubeMapFaceEnum _cubeMapFaceEnum;
        private readonly WrappedCubeTexture _wrappedCubeTexture;

        public CubeMapSurface(CubeMapFaceEnum cubeMapFaceEnum, WrappedCubeTexture wrappedCubeTexture)
        {
            _cubeMapFaceEnum = cubeMapFaceEnum;
            _wrappedCubeTexture = wrappedCubeTexture;
        }

        protected override void GetSurface(Device device)
        {
            Surface = _wrappedCubeTexture.CubeTexture.GetCubeMapSurface(WrappedCubeTexture.MapFace(_cubeMapFaceEnum), 0);
        }
    }
}