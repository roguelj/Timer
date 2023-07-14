using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace Timer.Shared.Application
{
    public static class ConfigurationServices
    {

        private static string? _userDataPath;
        public static string UserDataPath
        {
            get
            {
                return _userDataPath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationConstants.PUBLISHER, ApplicationConstants.APPLICATION_NAME);
            }

        }

        public static IConfiguration GetConfiguration()
        {

            var settingsFileName = ApplicationConstants.APP_SETTINGS;
            var exampleSettingsFileName = ApplicationConstants.APP_SETTINGS_EXAMPLE;
            var readMeFileName = ApplicationConstants.READ_ME_FILENAME;
            var workingDirectory = Directory.GetCurrentDirectory();
                

            // ensure the configuration directory exists
            if(!Directory.Exists(UserDataPath))
            {
                Directory.CreateDirectory(UserDataPath);
            }

            // ensure that the appsettings file exists. copy from working directory
            if (!File.Exists(Path.Combine(UserDataPath, settingsFileName)))
            {
                File.Copy(Path.Combine(workingDirectory, settingsFileName), Path.Combine(UserDataPath, settingsFileName));
            }


            // always copy appsettings over to the example file for the user to reference
            File.Copy(Path.Combine(workingDirectory, settingsFileName), Path.Combine(UserDataPath, exampleSettingsFileName), true);


            // always copy the README over, too
            File.Copy(Path.Combine(workingDirectory, readMeFileName), Path.Combine(UserDataPath, readMeFileName), true);


            // create and return config
            return new ConfigurationBuilder()
                        .SetBasePath(UserDataPath)
                        .AddJsonFile(settingsFileName, false, true)
                        .Build();

        }

    }

}
