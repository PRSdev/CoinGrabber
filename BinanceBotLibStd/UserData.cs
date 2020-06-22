using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BinanceBotLib
{
    public class UserData
    {
        public virtual string UserName { get; set; } = "New User";

        [Category("1 General"), Description("Binance API Key.")]
        public virtual string APIKey { get; set; }

        [Category("1 General"), Description("Binance Secret Key.")]
        public virtual string SecretKey { get; set; }

        public Settings Config { get; set; } = new Settings();

        public override string ToString()
        {
            return UserName;
        }
    }
}