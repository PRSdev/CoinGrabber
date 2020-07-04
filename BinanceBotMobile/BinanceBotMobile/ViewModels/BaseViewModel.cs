using BinanceBotLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
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
            Preferences.Set(prefName, value);
            OnPropertyChanged(prefName);
        }

        protected void SetPreference(string prefName, int value)
        {
            Preferences.Set(prefName, value);
            OnPropertyChanged(prefName);
        }
    }
}