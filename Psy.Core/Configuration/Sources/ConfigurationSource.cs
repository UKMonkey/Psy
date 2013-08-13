using System.Collections.Generic;

namespace Psy.Core.Configuration.Sources
{
    public abstract class ConfigurationSource
    {
        internal readonly Dictionary<string, Configuration> Configurations;

        public event NewConfigurationEvent NewConfiguration;

        private void OnNewConfiguration(Configuration configuration)
        {
            var handler = NewConfiguration;
            if (handler != null) 
                handler(this, new NewConfigurationEventArgs(configuration));
        }

        protected ConfigurationSource()
        {
            Configurations = new Dictionary<string, Configuration>(10);
        }

        public ConfigurationSource AddConfiguration(string name, int defaultValue)
        {
            if (Configurations.ContainsKey(name))
                return this;

            var configuration = new Configuration(name, defaultValue);
            Configurations[name] = configuration;
            OnNewConfiguration(configuration);
            return this;
        }

        public ConfigurationSource AddConfiguration(string name, float defaultValue)
        {
            if (Configurations.ContainsKey(name))
                return this;

            var configuration = new Configuration(name, defaultValue);
            Configurations[name] = configuration;
            OnNewConfiguration(configuration);
            return this;
        }

        public ConfigurationSource AddConfiguration(string name, string defaultValue)
        {
            if (Configurations.ContainsKey(name))
                return this;

            var configuration = new Configuration(name, defaultValue);
            Configurations[name] = configuration;
            OnNewConfiguration(configuration);
            return this;
        }

        public ConfigurationSource AddConfiguration(string name, bool defaultValue)
        {
            if (Configurations.ContainsKey(name))
                return this;

            var configuration = new Configuration(name, defaultValue);
            Configurations[name] = configuration;
            OnNewConfiguration(configuration);
            return this;
        }

        public abstract void ReadConfigurations();
        public abstract void WriteConfigurations();
    }
}