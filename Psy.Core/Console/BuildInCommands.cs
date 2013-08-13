using System;
using SlimMath;

namespace Psy.Core.Console
{
    public static class StandardCommands
    {
        private static void Help(params string[] parameters)
        {
            if (parameters.Length < 2)
                return;

            var command = StaticConsole.Console.CommandBindings.Get(parameters[1]);

            var aboutColor = new Color4(1.0f, 0.4f, 0.5f, 1.0f);

            if (command == null)
            {
                StaticConsole.Console.AddLine(String.Format("Unknown command {0}", parameters[1]), aboutColor);
                return;
            }
            StaticConsole.Console.AddLine(String.Format("Command: {0}", command.CommandName), aboutColor);
            StaticConsole.Console.AddLine(String.Format("Description: {0}", command.Description), aboutColor);
        }

        private static void About(params string[] parameters)
        {
            StaticConsole.Console.AddLine("Digitally imported radio!");
        }

        private static void List(params string[] parameters)
        {
            StaticConsole.Console.AddLine("Available commands:");
            foreach (var command in StaticConsole.Console.CommandBindings.BoundCommands)
            {
                StaticConsole.Console.AddLine(
                    String.Format("{0} - {1}",
                    command.CommandName, command.Description)
                    );
            }
            StaticConsole.Console.AddLine("-----------------------------");
        }

        public static void Attach()
        {
            StaticConsole.Console.CommandBindings.Bind("help", "Provide help on command", Help);
            StaticConsole.Console.CommandBindings.Bind("about", "Provide information on the console", About);
            StaticConsole.Console.CommandBindings.Bind("list", "List all available commands", List);
        }
    }
}
