using SlimMath;

namespace Psy.Core.Input
{
    public class MouseEventArguments
    {
        public Vector2 Position { get; set; }
        public MouseButton Button { get; set; }

        public MouseEventArguments() {}

        public MouseEventArguments(Vector2 position, MouseButton button)
        {
            Position = position;
            Button = button;
        }
    }
}
