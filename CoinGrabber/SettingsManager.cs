public static class SettingsManager
{
    public static readonly string PersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CoinGrabber");

    public static string LogFilePath
    {
        get
        {
            string logsFolder = Path.Combine(PersonalFolder, "Logs");
            string filename = string.Format("CoinGrabber-Log-{0:yyyy-MM}.log", DateTime.Now);
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
