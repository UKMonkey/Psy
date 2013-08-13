using System.Globalization;

namespace Psy.Core.Configuration
{
    public class Configuration
    {
        public readonly string Name;
        public readonly string DefaultValue;
        public string Value;

        public Configuration(string name, string defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
            Value = DefaultValue;
        }

        public Configuration(string name, int defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue.ToString(CultureInfo.InvariantCulture);
            Value = DefaultValue;
        }

        public Configuration(string name, bool defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue.ToString(CultureInfo.InvariantCulture);
            Value = DefaultValue;
        }

        public Configuration(string name, float defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue.ToString(CultureInfo.InvariantCulture);
            Value = DefaultValue;
        }
    }
}