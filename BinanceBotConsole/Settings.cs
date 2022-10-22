using ShareX.HelpersLib;
using System;

namespace BinanceBotLib
{
    public class Settings : SettingsBase<Settings>

    {
        public string APIKey { get; set; }

        public string SecretKey { get; set; }

        public DateTime CoinListingUtcTime { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}