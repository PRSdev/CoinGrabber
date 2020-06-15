using BinanceBot;
using BinanceBotLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BinanceBotMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsViewModel ViewModel
        {
            get => BindingContext as SettingsViewModel;
            set => BindingContext = value;
        }

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = new SettingsViewModel();
        }
    }
}