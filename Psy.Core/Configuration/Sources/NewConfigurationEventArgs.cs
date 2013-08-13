namespace Psy.Core.Configuration.Sources
{
    public class NewConfigurationEventArgs
    {
        public readonly Configuration Configuration;

        public NewConfigurationEventArgs(Configuration configuration)
        {
            Configuration = configuration;
        }
    }
}