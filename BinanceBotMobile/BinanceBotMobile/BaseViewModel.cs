using BinanceBotLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;

namespace BinanceBot
{
    public class BaseViewModel : Settings, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetPreference(string prefName, bool value)
        {
            Preferences.Set(prefName, value);
            OnPropertyChanged(prefName);
        }

        protected void SetPreference(string prefName, string value)
        {
            Preferences.Set(prefName, value);
            OnPropertyChanged(prefName);
        }

        protected void SetPreference(string prefName, double value)
        {
            Preferences.Set(prefName, (double)value);
            OnPropertyChanged(prefName);
        }
    }
}