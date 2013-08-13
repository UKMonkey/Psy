namespace Psy.Core.Input
{
    public class InputEvent
    {
        public readonly KeyAction KeyAction;
        public readonly Key Key;

        public InputEvent(Key key, KeyAction keyAction)
        {
            Key = key;
            KeyAction = keyAction;
        }
    }
}