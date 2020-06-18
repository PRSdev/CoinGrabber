using BinanceBot;
using BinanceBotLib;
using ExchangeClientLib;
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
        private int _count;

        public DashboardPage()
        {
            InitializeComponent();
            Title = $"BinanceBot {AppInfo.VersionString}";
        }

        private async void btnSettings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void btnStartStop_Clicked(object sender, EventArgs e)
        {
            Bot bot = new Bot(new SettingsViewModel() { BotMode = BotMode.Futures });
            bot.Start();

            bot.Strategy.TradeListItemHandled += Strategy_TradeListItemHandled;
            // var price = await bot.TestAsync();
            // lblStatus.Text = $"Price: ${price}";
        }

        private void Strategy_TradeListItemHandled(TradingData data)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Iteration: {_count++}");
            sb.AppendLine($"Market Price: {data.Price}");
            sb.AppendLine($"Entry Price: {data.BuyPriceAfterFees}");
            if (data.PriceLongBelow > 0)
            {
                sb.AppendLine($"Long Below: {data.PriceLongBelow}");
                sb.AppendLine($"Short Above: {data.PriceShortAbove}");
                sb.AppendLine($"Target Profit: {data.ProfitTarget}");
            }

            lblStatus.Dispatcher.BeginInvokeOnMainThread(new Action(() => { lblStatus.Text = sb.ToString(); }));
        }
    }
}