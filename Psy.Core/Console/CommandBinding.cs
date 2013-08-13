namespace Psy.Core.Console
{
    public class CommandBinding
    {
        public string CommandName { get; private set; }
        public string Description { get; private set; }
        public event ConsoleCommandDelegate CommandHandlers;

        internal CommandBinding(string commandName, string description)
        {
            CommandName = commandName;
            Description = description;
        }

        internal void InvokeCommandHandlers(params string[] parameters)
        {
            if (CommandHandlers != null)
            {
                CommandHandlers(parameters);
            }
        }
    }
}
