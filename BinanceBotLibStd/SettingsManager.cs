using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinanceBotLib
{
    public static class SettingsManager
    {
        public static readonly string PersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BinanceBot");
        public static UserProfiles UserProfiles { get; set; }

        public static string LogFilePath
        {
            get
            {
                string logsFolder = Path.Combine(PersonalFolder, "Logs");
                string filename = string.Format("BinanceBot-Log-{0:yyyy-MM}.log", DateTime.Now);
                return Path.Combine(logsFolder, filename);
            }
        }

        public static string SettingsFilePath
        {
            get
            {
                return Path.Combine(PersonalFolder, "Settings.json");
            }
        }

        public static string UserProfilesFilePath
        {
            get
            {
                return Path.Combine(PersonalFolder, "UserProfiles.json");
            }
        }

        public static void LoadUserProfiles()
        {
            if (File.Exists(UserProfilesFilePath))
            {
                UserProfiles = UserProfiles.Load(UserProfilesFilePath);
            }
            else if (File.Exists(SettingsFilePath))
            {
                // backward compatibility - remove at version 3.0
                UserProfiles = new UserProfiles();
                UserProfiles.Users.Add(new UserData());
                UserProfiles.Users[0].UserName = "Upgraded Profile";
                Settings oldSettings = LoadSettings();
                UserProfiles.Users[0].Config = oldSettings;
                UserProfiles.Users[0].APIKey = oldSettings.APIKey;
                UserProfiles.Users[0].SecretKey = oldSettings.SecretKey;
            }
        }

        public static void SaveUserSettings(UserProfiles settings)
        {
            if (settings != null)
            {
                settings.Save(SettingsFilePath);
            }
        }

        public static Settings LoadSettings()
        {
            return Settings.Load(SettingsFilePath);
        }

        public static void SaveSettings(Settings settings)
        {
            if (settings != null)
            {
                settings.Save(SettingsFilePath);
            }
        }
    }
}