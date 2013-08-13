using System;
using System.Collections.Generic;
using System.IO;
using Psy.Core.Logging;

namespace Psy.Core.Input
{
    public class InputBinder : IInputBinder
    {
        private readonly Dictionary<Key, string> _keyDownBinds;
        private readonly Dictionary<Key, string> _keyUpBinds;
        private readonly Dictionary<string, Action<InputEvent>> _handlers;

        private const char KeyDownMarker = '+';
        private const char KeyUpMarker = '-';

        public InputBinder()
        {
            _keyDownBinds = new Dictionary<Key, string>();
            _keyUpBinds = new Dictionary<Key, string>();
            _handlers = new Dictionary<string, Action<InputEvent>>();
        }

        public void SaveBindings(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                foreach (var item in _keyDownBinds)
                {
                    var output = string.Format("{0} {1}{2}", item.Key, KeyDownMarker, item.Value);
                    writer.WriteLine(output);
                }

                foreach (var item in _keyUpBinds)
                {
                    var output = string.Format("{0} {1}{2}", item.Key, KeyUpMarker, item.Value);
                    writer.WriteLine(output);
                }
            }
        }

        public bool LoadBindings(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                string input;
                while ((input = reader.ReadLine()) != null)
                {
                    if (input.StartsWith("#"))
                        continue;
                    if (!ParseBinding(input))
                    {
                        ResetHandlers();
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Parse a binding from a binding configuration file, expected format:
        /// 
        /// Mouse1 [+-]fire 
        /// </summary>
        /// <param name="input"></param>
        private bool ParseBinding(string input)
        {
            var parts = input.Split(' ');
            if (parts.Length != 2)
            {
                Logger.Write("Key bindings file in unexpected format");
                return false;
            }

            Key key;
            if (!Enum.TryParse(parts[0], true, out key))
            {
                Logger.Write(string.Format("Failed to parse keybinding. No such key `{0}`", parts[0]), LoggerLevel.Error);
                return false;
            }

            var action = parts[1];
            if (action[0] == KeyDownMarker)
                BindDown(action.TrimStart(KeyDownMarker), key);
            else if (action[0] == KeyUpMarker)
                BindUp(action.TrimStart(KeyUpMarker), key);
            else
            {
                Logger.Write(string.Format("Unrecognised format"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Register pressing a key to a particular action
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="action"></param>
        public InputBinder BindDown(string action, params Key[] keys)
        {
            foreach (var key in keys)
            {
                _keyDownBinds[key] = action;
            }
            return this;
        }

        /// <summary>
        /// Register releasing a key to a particular action
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="action"></param>
        public InputBinder BindUp(string action, params Key[] keys)
        {
            foreach (var key in keys)
            {
                _keyUpBinds[key] = action;
            }
            return this;
        }

        /// <summary>
        /// Register key down and key up to a particular action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keys"> </param>
        public InputBinder Bind(string action, params Key[] keys)
        {
            BindDown(action, keys);
            BindUp(action, keys);
            return this;
        }

        /// <summary>
        /// Register an action to an event handler. Can be chained.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handler"></param>
        public InputBinder Register(string action, Action<InputEvent> handler)
        {
            _handlers[action] = handler;
            return this;
        }

        public bool OnKeyDown(Key key)
        {
            if (!_keyDownBinds.ContainsKey(key))
            {
                _keyDownBinds[key] = "";
                return false;
            }

            var action = _keyDownBinds[key];

            if (!_handlers.ContainsKey(action))
            {
                _handlers[action] = delegate {  };
                return false;
            }

            var inputEvent = new InputEvent(key, KeyAction.Down);

            _handlers[action](inputEvent);

            return true;
        }

        public bool OnKeyUp(Key key)
        {
            if (!_keyUpBinds.ContainsKey(key))
            {
                _keyUpBinds[key] = "";
                return false;
            }

            var action = _keyUpBinds[key];

            if (!_handlers.ContainsKey(action))
            {
                _handlers[action] = delegate { };
                return false;
            }

            var inputEvent = new InputEvent(key, KeyAction.Up);

            _handlers[action](inputEvent);

            return true;
        }

        public void ResetBinds()
        {
            _keyDownBinds.Clear();
            _keyUpBinds.Clear();
        }

        public void ResetHandlers()
        {
            _handlers.Clear();
        }
    }

}