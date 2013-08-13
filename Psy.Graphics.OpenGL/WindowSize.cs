using OpenTK;
using Psy.Core;

namespace Psy.Graphics.OpenGL
{
    public class WindowSize : IWindowSize
    {
        private readonly GameWindow _gameWindow;

        public WindowSize(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
        }

        public int Width
        {
            get { return _gameWindow.Width; }
        }
        public int Height
        {
            get { return _gameWindow.Height; }
        }
    }
}