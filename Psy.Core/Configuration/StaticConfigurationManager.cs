namespace Psy.Core.Configuration
{
    public static class StaticConfigurationManager
    {
        public static ConfigurationManager ConfigurationManager;

        public static void Initialize()
        {
            ConfigurationManager = new ConfigurationManager();
        }
    }
}