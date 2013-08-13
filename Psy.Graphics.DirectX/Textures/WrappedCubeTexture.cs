using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Textures
{
    public class WrappedCubeTexture : ICubeTexture
    {
        internal CubeTexture CubeTexture;

        public string DebugName { get; set; }
        private readonly Pool _pool;
        private readonly int _edgeSize;
        private readonly Format _format;
        private readonly Usage _usage;
        public ISurface[] CubeMapSurface { get; set; }

        private WrappedCubeTexture(Device device, int edgeSize, Usage usage, Format format, Pool pool)
        {
            _usage = usage;
            _format = format;
            _edgeSize = edgeSize;
            _pool = pool;
            CreateCubeTexture(device);
        }

        private void CreateCubeTexture(Device device)
        {
            CubeTexture = new CubeTexture(device, _edgeSize, 1, _usage, _format, _pool);

            CubeMapSurface = new ISurface[6];
            for (var i = 0; i < 6; i++)
            {
                CubeMapSurface[i] = WrappedSurface.CubeMapSurface((CubeMapFaceEnum) i, this);
            }
        }

        internal void PreReset()
        {
            CubeTexture.Dispose();
        }

        internal void PostReset(Device device)
        {
            CreateCubeTexture(device);
        }

        public void Dispose()
        {
            CubeTexture.Dispose();
        }

        public static CubeMapFace MapFace(CubeMapFaceEnum face)
        {
            return (CubeMapFace) face;
        }

        public static WrappedCubeTexture Create(Device device, int edgeSize, UsageType usageType, FormatType formatType)
        {
            var usage = DirectXGraphicsContext.MapUsage(usageType);
            var format = DirectXGraphicsContext.MapFormat(formatType);

            var wrappedCubeTexture = new WrappedCubeTexture(device, edgeSize, usage, format, Pool.Default);

            return wrappedCubeTexture;
        }
    }
}