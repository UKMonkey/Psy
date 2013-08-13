namespace Psy.Core.Configuration.Sources
{
    public class AppConfigConfigurationSource : ConfigurationSource
    {
        public override void ReadConfigurations()
        {
            foreach (var configuration in Configurations.Values)
            {
                var appSetting = System.Configuration.ConfigurationManager.AppSettings[configuration.Name];
                if (!string.IsNullOrEmpty(appSetting))
                {
                    configuration.Value = appSetting;
                }
            }
        }

        public override void WriteConfigurations() { }
    }
}