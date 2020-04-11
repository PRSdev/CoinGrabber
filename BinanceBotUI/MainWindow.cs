using BinanceBotLib;
using Microsoft.Win32;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinanceBotUI
{
    public partial class MainWindow : Form
    {
        private bool IsReady = false;
        private RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public MainWindow()
        {
            InitializeComponent();

            this.Text = Application.ProductName;

            Bot.LoadSettings();

            foreach (BotMode botMode in Helpers.GetEnums<BotMode>())
            {
                cboBotMode.Items.Add(botMode.GetDescription());
            }
            cboBotMode.SelectedIndex = 1;

            foreach (CoinPair cp in Bot.CoinPairs)
            {
                cboNewDefaultCoinPair.Items.Add(cp);
            }
            cboNewDefaultCoinPair.SelectedIndex = Bot.CoinPairs.FindIndex(x => x.ToString() == Bot.Settings.CoinPair.ToString());

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

        private void UpdateUI()
        {
            lblProfitTotal.Text = "Profit made to-date: $" + Bot.Settings.TotalProfit;
        }

        private void Trade()
        {
            Bot.Started += TradingHelper_Started;
            Bot.PriceChecked += TradingHelper_PriceChecked;
            Bot.OrderSucceeded += TradingHelper_OrderSuccess;
            Bot.Completed += TradingHelper_Completed;

            Bot.Start();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            Trade();
        }

        private void TradingHelper_Completed()
        {
            UpdateUI();
        }

        private void TradingHelper_Started()
        {
            lvStatus.Items.Clear();
        }

        private void TradingHelper_PriceChecked(TradingData trade)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.SubItems.Add(trade.ID.ToString());
            lvi.SubItems.Add(trade.CoinPair.ToString());
            lvi.SubItems.Add(trade.BuyPriceAfterFees.ToString());
            lvi.SubItems.Add(trade.MarketPrice.ToString());
            lvi.SubItems.Add(trade.PriceChangePercentage.ToString());
            lvi.ForeColor = trade.PriceChangePercentage > 0m ? Color.Green : Color.Red;
            lvStatus.Items.Add(lvi);
        }

        private void TradingHelper_OrderSuccess(TradingData trade)
        {
            niTray.ShowBalloonTip(5000, Application.ProductName, trade.ToString(), ToolTipIcon.Info);
            lvStatus.Items.Add(trade.ToString());
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsWindow frm = new SettingsWindow();
            frm.ShowDialog();
        }

        private void cboNewDefaultCoinPair_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bot.Settings.CoinPair = cboNewDefaultCoinPair.SelectedItem as CoinPair;
        }

        private void cboBotMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bot.Settings.BotMode = (BotMode)cboBotMode.SelectedIndex;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Bot.SaveSettings();
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
    }
}