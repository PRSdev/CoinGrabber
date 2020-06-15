using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace BinanceBot
{
    public class SettingsViewModel : BaseViewModel
    {
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