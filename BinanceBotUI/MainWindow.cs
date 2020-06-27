using BinanceBotLib;
using ExchangeClientLib;
using Microsoft.Win32;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinanceBotUI
{
    public partial class MainWindow : Form
    {
        private bool IsReady = false;
        private bool IsBotRunning = false;

        private RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        #region Bot events

        private void Strategy_Completed()
        {
            this.InvokeSafe(() =>
            {
                UpdateUI();
            });
        }

        private void Strategy_Started()
        {
            this.InvokeSafe(() =>
            {
                lvStatus.Items.Clear();
            });
        }

        private void Strategy_Handled(TradingData trade)
        {
            this.InvokeSafe(() =>
            {
                lvStatus.Items.Add(trade.ToListViewItem()); // this updates UI

                lvStatistics.Items.Clear();
                // Futures
                if (trade.PriceLongBelow > 0) // this does not update UI
                {
                    AddStatistic($"Long Below", trade.PriceLongBelow.ToString());
                    AddStatistic($"Short Above", trade.PriceShortAbove.ToString());
                }

                if (trade.ProfitTarget > 0)
                {
                    AddStatistic("Target Profit", trade.ProfitTarget.ToString());
                }
            });
        }

        private void Strategy_OrderSuccess(TradingData trade)
        {
            this.InvokeSafe(() =>
            {
                niTray.ShowBalloonTip(5000, Application.ProductName, trade.ToString(), ToolTipIcon.Info);
            });
        }

        #endregion Bot events

        private void UpdateUI()
        {
            Text = $"{Application.ProductName} {Application.ProductVersion}";
            if (!Program.Bot.Settings.ProductionMode)
                Text += " - Simulation Mode";

            cboCoinPairDefaultNew.SelectedIndex = new CoinPairHelper(Program.Bot.Settings).GetCoinPairIndex();
            cboCoinPairDefaultNew.Enabled = !Program.Bot.Settings.RandomNewCoinPair && Program.Bot.Settings.BotMode != BotMode.TradingViewSignal;

            if (Program.Bot.Strategy != null)
            {
            NameValueCollection nvc = Program.Bot.Strategy.Statistics.GetReport();
            for (int i = 0; i < nvc.Count; i++)
            {
                AddStatistic(nvc.GetKey(i), nvc.Get(i));
            }
            }

        }

        private void AddStatistic(string name, string value)
        {
            ListViewItem lvi = new ListViewItem(name);
            lvi.SubItems.Add(value);
            lvStatistics.Items.Add(lvi);
        }

        private void Trade()
        {
            btnStartStop.Text = "Stop";

            Program.Bot.Start();

            Program.Bot.Strategy.Started += Strategy_Started;
            Program.Bot.Strategy.TradeListItemHandled += Strategy_Handled;
            Program.Bot.Strategy.OrderSucceeded += Strategy_OrderSuccess;
            Program.Bot.Strategy.Completed += Strategy_Completed;
        }

        public MainWindow()
        {
            InitializeComponent();

            foreach (BotMode botMode in Helpers.GetEnums<BotMode>())
            {
                cboBotMode.Items.Add(botMode.GetDescription());
            }
            cboBotMode.SelectedIndex = (int)Program.Bot.Settings.BotMode;

            foreach (CoinPair cp in ExchangeClient.CoinPairsList)
            {
                cboCoinPairDefaultNew.Items.Add(cp);
            }

            chkStartWithWindows.Checked = rkApp.GetValue(Application.ProductName) != null && rkApp.GetValue(Application.ProductName).Equals(Application.ExecutablePath);
            if (chkStartWithWindows.Checked)
            {
                Trade();
                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            IsBotRunning = !IsBotRunning;
            cboBotMode.Enabled = !IsBotRunning;

            if (IsBotRunning)
            {
                Trade();
            }
            else
            {
                Program.Bot.Stop();
                btnStartStop.Text = "Start";
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsWindow frm = new SettingsWindow();
            frm.ShowDialog();
            lvStatistics.Items.Clear();
            UpdateUI();
        }

        private void cboNewDefaultCoinPair_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Bot.Settings.CoinPair = cboCoinPairDefaultNew.SelectedItem as CoinPair;
        }

        private void cboBotMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Bot.Settings.BotMode = (BotMode)cboBotMode.SelectedIndex;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            BinanceBotLib.NativeMethods.AllowSleep();
            SettingsManager.SaveSettings(Program.Bot.Settings);
        }

        private void chkStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (IsReady)
            {
                if (chkStartWithWindows.Checked)
                {
                    rkApp.SetValue(Application.ProductName, Application.ExecutablePath);
                }
                else
                {
                    rkApp.DeleteValue(Application.ProductName, false);
                }
            }
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            IsReady = true;
        }

        private void niTray_DoubleClick(object sender, EventArgs e)
        {
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            Helpers.OpenFile(SettingsManager.LogFilePath);
        }
    }
}