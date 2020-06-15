using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace BinanceBot
{
    public class SettingsViewModel : BaseViewModel
    {
        public string APIKey
        {
            get => Preferences.Get(nameof(APIKey), "");
            set
            {
                Preferences.Set(nameof(APIKey), value);
                OnPropertyChanged(nameof(APIKey));
            }
        }

        public string SecretKey
        {
            get => Preferences.Get(nameof(SecretKey), "");
            set
            {
                Preferences.Set(nameof(SecretKey), value);
                OnPropertyChanged(nameof(SecretKey));
            }
        }

        // Futures

        public bool IsAutoAdjustShortAboveAndLongBelow
        {
            get => Preferences.Get(nameof(IsAutoAdjustShortAboveAndLongBelow), false);
            set
            {
                Preferences.Set(nameof(IsAutoAdjustShortAboveAndLongBelow), value);
                OnPropertyChanged(nameof(IsAutoAdjustShortAboveAndLongBelow));
            }
        }
    }
}