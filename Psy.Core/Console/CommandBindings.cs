using System;
using System.Collections.Generic;
using System.Linq;

namespace Psy.Core.Console
{
    public class CommandBindings
    {
        public class BoundCommandInfo
        {
            private readonly CommandBinding _commandBinding;
            public string CommandName
            {
                get { return _commandBinding.CommandName; }
            }

            public string Description
            {
                get { return _commandBinding.Description; }
            }

            public BoundCommandInfo(CommandBinding commandBinding)
            {
                _commandBinding = commandBinding;
            }
        }

        private readonly Dictionary<string, CommandBinding> _boundCommands;

        public IEnumerable<BoundCommandInfo> BoundCommands
        {
            get { return _boundCommands.Select(boundCommand => new BoundCommandInfo(boundCommand.Value)); }
        }

        public CommandBindings()
        {
            _boundCommands = new Dictionary<string, CommandBinding>();
        }

        private CommandBinding Bind(string commandName, string description)
        {
            if (commandName == "")
            {
                throw new ArgumentException("commandName cannot be blank.");
            }
            if (commandName.Contains(" "))
            {
                throw new ArgumentException("commandName cannot contain a space.");
            }

            var binding = new CommandBinding(commandName.ToLower(), description);
            _boundCommands[commandName.ToLower()] = binding;
            return binding;
        }

        public CommandBinding Get(string commandName)
        {
            return _boundCommands.ContainsKey(commandName) ? _boundCommands[commandName] : null;
        }

        /// <summary>
        /// Binds a command to a command handler. A command cannot contain spaces. When the handler
        /// is executed the first parameter is the command, the following parameters were parsed
        /// by splitting on the space.
        /// </summary>
        /// <param name="commandName">Name of the command. Spaces are not permitted. Case sensitive</param>
        /// <param name="description">Description of the command.</param>
        /// <param name="handler">Handler for the command.</param>
        /// <returns>A command binding instance.</returns>
        public CommandBinding Bind(string commandName, string description, ConsoleCommandDelegate handler)
        {
            var binding = Bind(commandName, description);
            binding.CommandHandlers += handler;
            return binding;
        }

        /// <summary>
        /// Parses a command and invokes the bound handler.
        /// </summary>
        /// <param name="commandString">Raw command string.</param>
        /// <returns>True if a handler was bound to the command.</returns>
        public bool Parse(string commandString)
        {
            if (commandString == "")
                return false;

            var commandParts = commandString.ParseArguments();

            if (_boundCommands.ContainsKey(commandParts[0]))
            {
                _boundCommands[commandParts[0]].InvokeCommandHandlers(commandParts);
                return true;
            }

            return false;
        }

        public void Remove(string command)
        {
            _boundCommands.Remove(command);
        }
    }
}
