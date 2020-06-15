using BinanceBot;
using BinanceBotLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BinanceBotMobile
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
            Title = $"BinanceBot {AppInfo.VersionString}";
        }

        private async void btnSettings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private void btnStartStop_Clicked(object sender, EventArgs e)
        {
            Settings botSettings = new Settings();
            SettingsViewModel viewModel = new SettingsViewModel();

            botSettings.APIKey = viewModel.APIKey;
            botSettings.SecretKey = viewModel.SecretKey;

            Bot bot = new Bot(botSettings);
            lblStatus.Text = $"Price: ${bot.Test()}";
        }
    }
}