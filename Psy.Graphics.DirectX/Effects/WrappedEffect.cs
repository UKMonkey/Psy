using Psy.Core;
using Psy.Graphics.DirectX.Textures;
using Psy.Graphics.Effects;
using SlimDX.Direct3D9;
using SlimMath;

namespace Psy.Graphics.DirectX.Effects
{
    public class WrappedEffect : IEffect
    {
        private readonly EffectCache _effectCache;
        public Effect Effect { get; set; }
        public string Filename { get; private set; }
        private TextureAreaHolder _texture;

        public WrappedEffect(EffectCache effectCache, Effect effect, string filename)
        {
            _effectCache = effectCache;
            Effect = effect;
            Filename = filename;
            _texture = null;
        }

        public IEffectHandle Technique 
        { 
            set 
            { 
                var wrappedEffectHandle = (WrappedEffectHandle) value;
                Effect.Technique = wrappedEffectHandle.EffectHandle;
            }
        }

        public IEffectHandle CreateHandle(string name)
        {
            return new WrappedEffectHandle(new EffectHandle(name));
        }

        public void SetValue<T>(string name, T value) where T : struct
        {
            Effect.SetValue(name, value);
        }

        public void SetValue<T>(IEffectHandle effectHandle, T value) where T : struct
        {
            var wrappedEffectHandle = (WrappedEffectHandle) effectHandle;
            Effect.SetValue(wrappedEffectHandle.EffectHandle, value);
        }

        public void SetMatrix(string name, Matrix value)
        {
            Effect.SetValue(name, value);
        }

        public void SetMatrix(IEffectHandle effectHandle, Matrix value)
        {
            var wrappedEffectHandle = (WrappedEffectHandle)effectHandle;
            Effect.SetValue(wrappedEffectHandle.EffectHandle, value);
        }

        private void SetTexture(string name, Texture texture)
        {
            Effect.SetTexture(name, texture);
        }

        private void SetTexture(IEffectHandle effectHandle, Texture texture)
        {
            var wrappedEffectHandle = (WrappedEffectHandle)effectHandle;
            Effect.SetTexture(wrappedEffectHandle.EffectHandle, texture);
        }

        public void SetTexture(string name, CubeTexture texture)
        {
            Effect.SetTexture(name, texture);
        }

        public void SetTexture(IEffectHandle effectHandle, CubeTexture texture)
        {
            var wrappedEffectHandle = (WrappedEffectHandle) effectHandle;
            Effect.SetTexture(wrappedEffectHandle.EffectHandle, texture);
        }

        public void SetTexture(string name, TextureAreaHolder texture)
        {
            var cachedTexture = (CachedTexture) texture.TextureArea;
            SetTexture(name, cachedTexture.Texture);
        }

        public void SetTexture(IEffectHandle effectHandle, TextureAreaHolder texture)
        {
            var cachedTexture = (CachedTexture)texture.TextureArea;
            SetTexture(effectHandle, cachedTexture.Texture);
        }

        public void SetTexture(IEffectHandle effectHandle, ICubeTexture cubeTexture)
        {
            var wrappedEffectHandle = (WrappedEffectHandle) effectHandle;
            Effect.SetTexture(wrappedEffectHandle.EffectHandle, ((WrappedCubeTexture) cubeTexture).CubeTexture);
        }

        public void SetTexture(string name, ICubeTexture cubeTexture)
        {
            Effect.SetTexture(name, ((WrappedCubeTexture)cubeTexture).CubeTexture);
        }

        public void Begin()
        {
            Effect.Begin(FX.None);
        }

        public void BeginPass(int i)
        {
            Effect.BeginPass(i);
        }

        public void EndPass()
        {
            Effect.EndPass();
        }

        public void End()
        {
            Effect.End();
        }

        public void CommitChanges()
        {
            Effect.CommitChanges();
        }

        public void Reload()
        {
            _effectCache.Reload(this);
        }
    }
}