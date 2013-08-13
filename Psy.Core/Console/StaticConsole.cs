namespace Psy.Core.Console
{
    public static class StaticConsole
    {
        public static BaseConsole Console;
        public static void Initialize()
        {
            Console = new BaseConsole();
        }
    }
}