using System.IO;

namespace Psy.Core.Configuration.Sources
{
    public class FileConfigurationSource : ConfigurationSource
    {
        private string Filepath { get; set; }

        public FileConfigurationSource(string filepath)
        {
            Filepath = filepath;
        }

        public override void ReadConfigurations()
        {
            if (!File.Exists(Filepath))
            {
                return;
            }

            var fileContent = File.ReadAllLines(Filepath);

            foreach (var line in fileContent)
            {
                var separatorIndex = line.IndexOf('=');
                var name = line.Substring(0, separatorIndex);
                var value = line.Substring(separatorIndex+1);

                if (Configurations.ContainsKey(name))
                {
                    Configurations[name].Value = value;
                }
                else
                {
                    Configurations[name] = new Configuration(name, value);
                }
            }
        }

        public override void WriteConfigurations()
        {
            using (var fileHandle = new StreamWriter(Filepath))
            {
                foreach (var configuration in Configurations)
                {
                    fileHandle.WriteLine("{0}={1}", configuration.Value.Name, configuration.Value.Value);
                }
            }
        }
        
    }
}