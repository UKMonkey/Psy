using System;
using System.IO;

namespace Psy.Core.Input
{
    public interface IInputBinder {

        void SaveBindings(Stream stream);
        bool LoadBindings(Stream stream);

        /// <summary>
        /// Register pressing a key to a particular action
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="action"></param>
        InputBinder BindDown(string action, params Key[] keys);

        /// <summary>
        /// Register releasing a key to a particular action
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="action"></param>
        InputBinder BindUp(string action, params Key[] keys);

        /// <summary>
        /// Register key down and key up to a particular action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keys"> </param>
        InputBinder Bind(string action, params Key[] keys);

        /// <summary>
        /// Register an action to an event handler
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handler"></param>
        InputBinder Register(string action, Action<InputEvent> handler);

        bool OnKeyDown(Key key);
        bool OnKeyUp(Key key);

        /// <summary>
        /// Reset all BindDown(...) BindUp(...) actions
        /// </summary>
        void ResetBinds();

        /// <summary>
        /// Reset all RegisterHandler(...) actions
        /// </summary>
        void ResetHandlers();
    }
}