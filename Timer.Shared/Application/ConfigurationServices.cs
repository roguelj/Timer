using Microsoft.Extensions.Configuration;

namespace Timer.Shared.Application
{
    public static class ConfigurationServices
    {

        public static IConfiguration GetConfiguration()
        {

            var configBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "roguelj", "Timer"); // TODO: detect and act
            var settingsFileName = "appsettings.json";
            var workingDirectory = Directory.GetCurrentDirectory();
                        

            // check that the file exists
            if (!File.Exists(Path.Combine(configBasePath, settingsFileName)))
            {
                Directory.CreateDirectory(configBasePath);
                File.Copy(Path.Combine(workingDirectory, settingsFileName), Path.Combine(configBasePath, settingsFileName));
            }


            // create and return config
            return new ConfigurationBuilder()
                        .SetBasePath(configBasePath)
                        .AddJsonFile(settingsFileName)
                        .Build();

        }

    }

}
