using System;
using System.Collections.Generic;
using Psy.Core.Console;
using Psy.Core.FileSystem;
using Psy.Core.Logging;
using Psy.Graphics.Effects;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Effects
{
    public class EffectCache
    {
        private readonly Device _device;
        private readonly Dictionary<string, Effect> _effects;
        private readonly List<WrappedEffect> _wrappedEffects; 

        public EffectCache(Device device)
        {
            _device = device;
            _effects = new Dictionary<string, Effect>();
            _wrappedEffects = new List<WrappedEffect>();

            BindConsoleCommands();
        }

        private void BindConsoleCommands()
        {
            StaticConsole.Console.CommandBindings.Remove("dx_reload_fx");
            StaticConsole.Console.CommandBindings.Bind("dx_reload_fx", "Reload all effect shaders", ReloadFxHandler);
        }

        private void ReloadFxHandler(params string[] parameters)
        {
            foreach (var wrappedEffect in _wrappedEffects)
            {
                wrappedEffect.Reload();
            }
            Logger.Write(string.Format("dx_reload_fx: Reloaded {0} effects", _wrappedEffects.Count), LoggerLevel.Info);
        }

        public void DevicePreReset()
        {
            foreach (var effect in _effects)
            {
                effect.Value.OnLostDevice();
            }
        }

        public void DevicePostReset()
        {
            foreach (var effect in _effects)
            {
                effect.Value.OnResetDevice();
            }
        }

        public IEffect GetEffect(string filename)
        {
            if (!_effects.ContainsKey(filename))
            {
                _effects[filename] = LoadEffect(filename);
            }

            var wrappedEffect = new WrappedEffect(this, _effects[filename], filename);

            _wrappedEffects.Add(wrappedEffect);

            return wrappedEffect;
        }

        private Effect LoadEffect(string filename)
        {
            return Effect.FromFile(_device, Lookup.GetAssetPath(filename), ShaderFlags.None);
        }

        public void Reload(WrappedEffect wrappedEffect)
        {
            Effect newEffect;

            try
            {
                newEffect = LoadEffect(wrappedEffect.Filename);
            }
            catch (SlimDX.CompilationException compilationException)
            {
                Logger.WriteException(compilationException);
                Logger.Write("Shader reload was aborted.", LoggerLevel.Warning);
                return;
            }

            _effects.Remove(wrappedEffect.Filename);
            wrappedEffect.Effect.Dispose();

            _effects.Add(wrappedEffect.Filename, newEffect);

            wrappedEffect.Effect = newEffect;
        }
    }
}