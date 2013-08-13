using System;

namespace Psy.Graphics.DirectX
{
    public static class TextureFilterMapper
    {
        public static TextureFilter MapFrom(SlimDX.Direct3D9.TextureFilter textureFilter)
        {
            switch (textureFilter)
            {
                case SlimDX.Direct3D9.TextureFilter.None:
                    return TextureFilter.None;

                case SlimDX.Direct3D9.TextureFilter.Anisotropic:
                    return TextureFilter.Anisotropic;

                case SlimDX.Direct3D9.TextureFilter.Linear:
                    return TextureFilter.Linear;

                case SlimDX.Direct3D9.TextureFilter.Point:
                    return TextureFilter.Point;

                default:
                    throw new ArgumentOutOfRangeException("textureFilter");
            }
        }

        public static SlimDX.Direct3D9.TextureFilter MapFrom(TextureFilter textureFilter)
        {
            switch (textureFilter)
            {
                case TextureFilter.None:
                    return SlimDX.Direct3D9.TextureFilter.None;

                case TextureFilter.Linear:
                    return SlimDX.Direct3D9.TextureFilter.Linear;

                case TextureFilter.Anisotropic:
                    return SlimDX.Direct3D9.TextureFilter.Linear;

                case TextureFilter.Point:
                    return SlimDX.Direct3D9.TextureFilter.Point;

                default:
                    throw new ArgumentOutOfRangeException("textureFilter");
            }
        }
    }
}