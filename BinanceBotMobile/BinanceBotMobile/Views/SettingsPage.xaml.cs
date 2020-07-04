using BinanceBot;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BinanceBotMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }
    }
}