using Psy.Core.Input;
using SlimMath;

namespace Psy.Core.Console
{
    public interface IConsole
    {
        CommandBindings CommandBindings { get; }

        void AddLine(string text);
        void AddLine(string text, Color4 colour);
        void OnKeyPress(KeyPressEventArguments keyPressEventArgs);
        void Eval(string input);
    }
}
