using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Textures
{
    public class WrappedTexture : ITexture
    {
        private Texture _texture;
        private readonly int _width;
        private readonly int _height;
        private readonly UsageType _usageType;
        private readonly FormatType _formatType;

        private WrappedTexture(Device device, int width, int height, UsageType usageType, FormatType formatType)
        {
            _width = width;
            _height = height;
            _usageType = usageType;
            _formatType = formatType;
            CreateTexture(device);
        }

        private void CreateTexture(Device device)
        {
            _texture = new Texture(
                device, _width, _height, 1, 
                DirectXGraphicsContext.MapUsage(_usageType), 
                DirectXGraphicsContext.MapFormat(_formatType), 
                Pool.Default);
        }

        public void Dispose()
        {
            _texture.Dispose();
        }

        public ISurface Surface
        {
            get
            {
                return WrappedSurface.TextureSurface(_texture, 0);
            }
        }

        public string DebugName { get; set; }

        internal void PreReset()
        {
            _texture.Dispose();
        }

        internal void PostReset(Device device)
        {
            CreateTexture(device);
        }

        public static WrappedTexture Create(Device device, int width, int height, UsageType usageType, FormatType formatType)
        {
            return new WrappedTexture(device, width, height, usageType, formatType);
        }
    }
}