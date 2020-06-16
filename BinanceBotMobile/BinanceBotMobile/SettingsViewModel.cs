using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace BinanceBot
{
    public class SettingsViewModel : BaseViewModel
    {
        public override string APIKey
        {
            get => Preferences.Get(nameof(APIKey), "");
            set { SetPreference(nameof(APIKey), value); }
        }

        public override string SecretKey
        {
            get => Preferences.Get(nameof(SecretKey), "");
            set { SetPreference(nameof(SecretKey), value); }
        }

        // Futures

        public override bool IsAutoAdjustShortAboveAndLongBelow
        {
            get => Preferences.Get(nameof(IsAutoAdjustShortAboveAndLongBelow), false);
            set { SetPreference(nameof(IsAutoAdjustShortAboveAndLongBelow), value); }
        }
    }
}