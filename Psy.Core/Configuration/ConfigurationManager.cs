using System.Collections.Generic;
using System.Globalization;
using Psy.Core.Configuration.Sources;

namespace Psy.Core.Configuration
{
    public class ConfigurationManager
    {
        private readonly Dictionary<string, string> _values;
        private readonly List<ConfigurationSource> _sources;

        public ConfigurationManager()
        {
            _values = new Dictionary<string, string>(20);
            _sources = new List<ConfigurationSource>(10);
        }

        public void RegisterSource(ConfigurationSource source)
        {
            _sources.Add(source);
            source.NewConfiguration += SourceOnNewConfiguration;
            source.ReadConfigurations();
            PullConfigurationValues(source);
        }

        private void SourceOnNewConfiguration(object sender, NewConfigurationEventArgs args)
        {
            _values[args.Configuration.Name] = args.Configuration.Value;
        }

        private void PullConfigurationValues(ConfigurationSource source)
        {
            foreach (var configuration in source.Configurations)
            {
                _values[configuration.Value.Name] = configuration.Value.Value;
            }
        }

        public void WriteSources()
        {
            foreach (var source in _sources)
            {
                PushConfigurationValues(source);
                source.WriteConfigurations();
            }
        }

        private void PushConfigurationValues(ConfigurationSource source)
        {
            foreach (var configuration in source.Configurations)
            {
                configuration.Value.Value = _values[configuration.Value.Name];
            }
        }

        public string GetString(string name)
        {
            if (!_values.ContainsKey(name))
                return "";

            return _values[name];
        }

        public float GetFloat(string name)
        {
            return float.Parse(_values[name]);
        }

        public int GetInt(string name)
        {
            return int.Parse(_values[name]);
        }

        public bool GetBool(string name)
        {
            return bool.Parse(_values[name]);
        }

        public void SetString(string name, string value)
        {
            _values[name] = value;
        }

        public void SetFloat(string name, float value)
        {
            _values[name] = value.ToString(CultureInfo.InvariantCulture);
        }

        public void SetInt(string name, int value)
        {
            _values[name] = value.ToString(CultureInfo.InvariantCulture);
        }

        public void SetBool(string name, bool value)
        {
            _values[name] = value.ToString();
        }
    }
}