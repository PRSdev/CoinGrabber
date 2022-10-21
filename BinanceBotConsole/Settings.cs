using ShareX.HelpersLib;
using System.ComponentModel;

namespace BinanceBotLib
{
    public class Settings : SettingsBase<Settings>

    {
        [Category("1 General"), Description("Binance API Key.")]
        public virtual string APIKey { get; set; }

        [Category("1 General"), Description("Binance Secret Key.")]
        public virtual string SecretKey { get; set; }
    }
}