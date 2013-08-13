using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Psy.Core.Console;
using Psy.Core.Logging;

namespace Psy.Windows
{
    public class DebugKeyHandler
    {
        private readonly Dictionary<Keys, string> _keybindings;

        public DebugKeyHandler()
        {
            BindConsoleCommands();
            _keybindings = new Dictionary<Keys, string>();
        }

        public bool HandleKey(Keys key)
        {
            if (!_keybindings.ContainsKey(key))
            {
                return false;
            }

            StaticConsole.Console.Eval(_keybindings[key]);

            return true;
        }

        private void BindConsoleCommands()
        {
            var commandBindings = StaticConsole.Console.CommandBindings;

            commandBindings.Bind("bindkey", "Bind Ctrl+key to a debug command `bindkey F1 command_here", HandleBindKey);
            commandBindings.Bind("unbindkey", "Unbind a key", HandleUnbindKey);
            commandBindings.Bind("unbindall", "Unbind all keys", HandleUnbindAll);
            commandBindings.Bind("listbinds", "List all active key binds", HandleListBinds);
        }

        private void HandleListBinds(string[] parameters)
        {
            foreach (var keybinding in _keybindings)
            {
                StaticConsole.Console.AddLine(string.Format("{0} -> {1}", keybinding.Key, keybinding.Value));
            }
        }

        private void HandleUnbindAll(string[] parameters)
        {
            _keybindings.Clear();
        }

        private void HandleUnbindKey(string[] parameters)
        {
            if (parameters.Length < 2)
                return;

            var keyName = parameters[1];
            Keys key;
            if (!Enum.TryParse(keyName, true, out key))
            {
                Logger.Write(string.Format("Cannot bind unknown key `{0}`", key), LoggerLevel.Warning);
                return;
            }

            if (!_keybindings.ContainsKey(key))
            {
                Logger.Write(string.Format("Key `{0}` is not bound to a command", key), LoggerLevel.Warning);
            }

            _keybindings.Remove(key);
        }

        private void HandleBindKey(params string[] parameters)
        {
            if (parameters.Length < 3)
                return;
            
            var command = parameters[2];

            var keyName = parameters[1];
            Keys key;
            if (!Enum.TryParse(keyName, true, out key))
            {
                Logger.Write(string.Format("Cannot bind command `{0}` to unknown key `{1}`", command, key), LoggerLevel.Warning);
                return;
            }

            _keybindings[key] = command;
        }
    }
}