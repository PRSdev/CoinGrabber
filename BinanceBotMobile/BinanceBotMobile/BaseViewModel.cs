using BinanceBotLib;
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