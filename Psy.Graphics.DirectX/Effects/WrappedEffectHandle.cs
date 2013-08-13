using Psy.Graphics.Effects;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Effects
{
    public class WrappedEffectHandle : IEffectHandle
    {
        public EffectHandle EffectHandle { get; set; }

        public WrappedEffectHandle(EffectHandle effectHandle)
        {
            EffectHandle = effectHandle;
        }
    }
}