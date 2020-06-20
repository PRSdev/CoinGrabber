using BinanceBotLib;
using ExchangeClientLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BinanceBotWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsReady = false;
        private bool IsBotRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            foreach (BotMode botMode in Helpers.GetEnums<BotMode>())
            {
                cboBotMode.Items.Add(botMode.GetDescription());
            }
            cboBotMode.SelectedIndex = (int)App.Bot.Settings.BotMode;

            IsAutoAdjustShortAboveAndLongBelow.IsChecked = App.Bot.Settings.IsAutoAdjustShortAboveAndLongBelow;
        }

        private void Trade()
        {
            btnStartStop.Content = "Stop";

            App.Bot.Start();

            App.Bot.Strategy.Started += Strategy_Started;
            App.Bot.Strategy.TradeListItemHandled += Strategy_TradeListItemHandled;
        }

        private void Strategy_TradeListItemHandled(TradingData data)
        {
            txtStatus.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { txtStatus.Text = App.Bot.ToStatusString(data); }));
        }

        private void Strategy_Started()
        {
            // throw new NotImplementedException();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            IsBotRunning = !IsBotRunning;
            cboBotMode.IsEnabled = !IsBotRunning;

            if (IsBotRunning)
            {
                Trade();
            }
            else
            {
                App.Bot.Stop();
                btnStartStop.Content = "Start";
            }
        }

        private void IsAutoAdjustShortAboveAndLongBelow_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Bot.SaveSettings(App.Bot.Settings);
        }

        private void Main_ContentRendered(object sender, EventArgs e)
        {
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            IsReady = true;
        }

        private void IsAutoAdjustShortAboveAndLongBelow_Click(object sender, RoutedEventArgs e)
        {
            App.Bot.Settings.IsAutoAdjustShortAboveAndLongBelow = IsAutoAdjustShortAboveAndLongBelow.IsChecked.Value;
        }
    }
}