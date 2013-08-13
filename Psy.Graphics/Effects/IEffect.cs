using Psy.Core;
using SlimMath;

namespace Psy.Graphics.Effects
{
    public interface IEffect
    {
        IEffectHandle Technique { set; }

        IEffectHandle CreateHandle(string name);

        void SetValue<T>(IEffectHandle effectHandle, T value) where T : struct;
        void SetMatrix(string name, Matrix value);
        void SetMatrix(IEffectHandle effectHandle, Matrix value);
        void SetTexture(IEffectHandle effectHandle, ICubeTexture texture);
        void SetTexture(string name, TextureAreaHolder texture);
        void SetTexture(IEffectHandle effectHandle, TextureAreaHolder texture);
        void SetTexture(string name, ICubeTexture cubeTexture);
        void Begin();
        void BeginPass(int i);
        void EndPass();
        void End();
        void CommitChanges();
        void Reload();
    }
}