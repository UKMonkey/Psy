using System;
using System.Collections.Generic;
using Psy.Core.Input;
using SlimMath;

namespace Psy.Core.Console
{
    public class BaseConsole : IConsole
    {
        public readonly List<ConsoleLine> ConsoleLines;
        public string InputLine { get; set; }
        public readonly int BufferSize;
        private readonly Dictionary<string, FloatVariable> _floatVariables;
        private readonly List<string> _previousCommands;
        private int _previousCommandIndex;

        public CommandBindings CommandBindings { get; protected set; }

        public BaseConsole(int bufferSize = 1000)
        {
            CommandBindings = new CommandBindings();
            ConsoleLines = new List<ConsoleLine>(bufferSize);
            BufferSize = bufferSize;
            _floatVariables = new Dictionary<string, FloatVariable>();
            _previousCommands = new List<string>(10);
            _previousCommandIndex = 0;
        }

        public void RegisterFloat(string name, Func<float> getValue, Action<float> setValue)
        {
            _floatVariables[name] = new FloatVariable { GetValue = getValue, SetValue = setValue};
        }

        private FloatVariable GetVariable(string varName)
        {
            return !_floatVariables.ContainsKey(varName) ? null : _floatVariables[varName];
        }

        public void AddLine(string text, Color4 colour)
        {
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var consoleLine = new ConsoleLine(line, colour);
                AddConsoleLine(consoleLine);
            }
        }

        public void AddLine(string text)
        {
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var consoleLine = new ConsoleLine(line);
                AddConsoleLine(consoleLine);
            }
        }

        public void OnKeyPress(KeyPressEventArguments keyPressEventArgs)
        {
            if (keyPressEventArgs.KeyChar < 32 || keyPressEventArgs.KeyChar > 126)
            {
                switch (keyPressEventArgs.KeyChar)
                {
                    case '\b':
                        if (InputLine.Length > 0)
                        {
                            InputLine = InputLine.Remove(InputLine.Length - 1);
                        }
                        break;
                    case '\r':
                    case '\n':
                        ParseLine();
                        break;
                }
            }
            else
            {
                InputLine += keyPressEventArgs.KeyChar;
            }
        }

        public void OnKeyDown(Key key)
        {
            switch (key)
            {
                case Key.Up:
                    if (_previousCommands.Count > 0)
                    {
                        _previousCommandIndex--;

                        if (_previousCommandIndex < 0)
                        {
                            _previousCommandIndex = _previousCommands.Count-1;
                        }

                        InputLine = _previousCommands[_previousCommandIndex];
                    }
                    
                    break;

                case Key.Down:
                    if (_previousCommands.Count > 0)
                    {
                        _previousCommandIndex++;

                        if (_previousCommandIndex > _previousCommands.Count-1)
                        {
                            _previousCommandIndex = 0;
                        }

                        InputLine = _previousCommands[_previousCommandIndex];    
                    }
                    
                    break;
            }
        }

        private void AddConsoleLine(ConsoleLine consoleLine)
        {
            lock (this)
            {
                ConsoleLines.Add(consoleLine);
                if (ConsoleLines.Count > BufferSize)
                {
                    // remove the oldest line.
                    ConsoleLines.RemoveAt(0);
                }
            }
        }

        private void ParseLine()
        {
            if (string.IsNullOrEmpty(InputLine))
                return;

            AddLine(">" + InputLine, new Color4(1.0f, 0.5f, 0.5f, 0.5f));

            _previousCommands.Add(InputLine);
            _previousCommandIndex = -1;

            Eval(InputLine);
            InputLine = "";
        }

        /// <summary>
        /// Execute the specified input.
        /// </summary>
        /// <param name="input">Either a command or cvar</param>
        public void Eval(string input)
        {
            if (input.Length <= 0)
                return;

            if (ParseCommand(input))
                return;

            if (!ParseCVar(input))
            {
                AddLine(
                    string.Format("I don't know how to '{0}'", input),
                    new Color4(1.0f, 08f, 0.2f, 0.2f));
            }
        }

        private bool ParseCommand(string input)
        {
            return CommandBindings.Parse(input);
        }

        private bool ParseCVar(string input)
        {
            var parts = input.ParseArguments();
            if (parts.Length < 1)
            {
                return false;
            }

            var varName = parts[0];
            var consoleVariable = GetVariable(varName);
            if (consoleVariable == null)
                return false;

            if (parts.Length == 1)
            {
                // show variable contents
                AddLine(string.Format("{0} = {1}", varName, consoleVariable.GetValue()));
            }
            else
            {
                var val = 0.0f;
                
                if (!float.TryParse(parts[1], out val))
                {
                    return false;
                }

                // set variable
                consoleVariable.SetValue(val);
                AddLine(string.Format("{0} = {1}", varName, val), new Color4(1.0f, 0.8f, 0.2f, 0.3f));
            }

            return true;
        }

        /// <summary>
        /// Display details of the exception.
        /// </summary>
        /// <param name="exception"></param>
        public void AddException(Exception exception)
        {
            AddLine("Exception details:");
            AddLine(exception.Message, Colours.Red);
            AddLine(exception.GetType().Name, Colours.Red);
            AddLine(exception.StackTrace, Colours.Red);
        }
    }
}