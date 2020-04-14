using ShareX.HelpersLib;

namespace BinanceBotUI
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Total Profit earned to-date");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Profit per day");
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.cboBotMode = new System.Windows.Forms.ComboBox();
            this.gbBotMode = new System.Windows.Forms.GroupBox();
            this.gbCoinPair = new System.Windows.Forms.GroupBox();
            this.cboCoinPairDefaultNew = new System.Windows.Forms.ComboBox();
            this.lvStatus = new ShareX.HelpersLib.MyListView();
            this.chID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chQuantity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCoinPair = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chBuyPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMarketPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPriceChangePerc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkStartWithWindows = new System.Windows.Forms.CheckBox();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.flpTopRight = new System.Windows.Forms.FlowLayoutPanel();
            this.flpTopLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.lvStatistics = new ShareX.HelpersLib.MyListView();
            this.chStatistic = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chStatisticValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCost = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbBotMode.SuspendLayout();
            this.gbCoinPair.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.flpTopRight.SuspendLayout();
            this.flpTopLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // niTray
            // 
            this.niTray.Icon = ((System.Drawing.Icon)(resources.GetObject("niTray.Icon")));
            this.niTray.Visible = true;
            this.niTray.DoubleClick += new System.EventHandler(this.niTray_DoubleClick);
            // 
            // btnStartStop
            // 
            this.btnStartStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartStop.Location = new System.Drawing.Point(2, 23);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(117, 31);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSettings.Location = new System.Drawing.Point(2, 58);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(2);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(117, 31);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "Settings...";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // cboBotMode
            // 
            this.cboBotMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBotMode.FormattingEnabled = true;
            this.cboBotMode.Location = new System.Drawing.Point(11, 21);
            this.cboBotMode.Margin = new System.Windows.Forms.Padding(2);
            this.cboBotMode.Name = "cboBotMode";
            this.cboBotMode.Size = new System.Drawing.Size(156, 21);
            this.cboBotMode.TabIndex = 3;
            this.cboBotMode.SelectedIndexChanged += new System.EventHandler(this.cboBotMode_SelectedIndexChanged);
            // 
            // gbBotMode
            // 
            this.gbBotMode.Controls.Add(this.cboBotMode);
            this.gbBotMode.Location = new System.Drawing.Point(2, 2);
            this.gbBotMode.Margin = new System.Windows.Forms.Padding(2);
            this.gbBotMode.Name = "gbBotMode";
            this.gbBotMode.Padding = new System.Windows.Forms.Padding(2);
            this.gbBotMode.Size = new System.Drawing.Size(197, 52);
            this.gbBotMode.TabIndex = 4;
            this.gbBotMode.TabStop = false;
            this.gbBotMode.Text = "Trading strategy";
            // 
            // gbCoinPair
            // 
            this.gbCoinPair.Controls.Add(this.cboCoinPairDefaultNew);
            this.gbCoinPair.Location = new System.Drawing.Point(2, 58);
            this.gbCoinPair.Margin = new System.Windows.Forms.Padding(2);
            this.gbCoinPair.Name = "gbCoinPair";
            this.gbCoinPair.Padding = new System.Windows.Forms.Padding(2);
            this.gbCoinPair.Size = new System.Drawing.Size(197, 52);
            this.gbCoinPair.TabIndex = 5;
            this.gbCoinPair.TabStop = false;
            this.gbCoinPair.Text = "Coin Pair for new trades";
            // 
            // cboCoinPairDefaultNew
            // 
            this.cboCoinPairDefaultNew.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCoinPairDefaultNew.FormattingEnabled = true;
            this.cboCoinPairDefaultNew.Location = new System.Drawing.Point(11, 21);
            this.cboCoinPairDefaultNew.Margin = new System.Windows.Forms.Padding(2);
            this.cboCoinPairDefaultNew.Name = "cboCoinPairDefaultNew";
            this.cboCoinPairDefaultNew.Size = new System.Drawing.Size(156, 21);
            this.cboCoinPairDefaultNew.TabIndex = 3;
            this.cboCoinPairDefaultNew.SelectedIndexChanged += new System.EventHandler(this.cboNewDefaultCoinPair_SelectedIndexChanged);
            // 
            // lvStatus
            // 
            this.lvStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chID,
            this.chQuantity,
            this.chCoinPair,
            this.chBuyPrice,
            this.chCost,
            this.chMarketPrice,
            this.chPriceChangePerc});
            this.lvStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvStatus.FullRowSelect = true;
            this.lvStatus.HideSelection = false;
            this.lvStatus.Location = new System.Drawing.Point(2, 142);
            this.lvStatus.Margin = new System.Windows.Forms.Padding(2);
            this.lvStatus.Name = "lvStatus";
            this.lvStatus.Size = new System.Drawing.Size(248, 137);
            this.lvStatus.TabIndex = 6;
            this.lvStatus.UseCompatibleStateImageBehavior = false;
            this.lvStatus.View = System.Windows.Forms.View.Details;
            // 
            // chID
            // 
            this.chID.Text = "ID";
            this.chID.Width = 33;
            // 
            // chQuantity
            // 
            this.chQuantity.Text = "Quantity";
            this.chQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chQuantity.Width = 80;
            // 
            // chCoinPair
            // 
            this.chCoinPair.Text = "Coin Pair";
            this.chCoinPair.Width = 80;
            // 
            // chBuyPrice
            // 
            this.chBuyPrice.Text = "Buy Price ($)";
            this.chBuyPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chBuyPrice.Width = 75;
            // 
            // chMarketPrice
            // 
            this.chMarketPrice.Text = "Market Price ($)";
            this.chMarketPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chMarketPrice.Width = 90;
            // 
            // chPriceChangePerc
            // 
            this.chPriceChangePerc.Text = "Price Change (%)";
            this.chPriceChangePerc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chPriceChangePerc.Width = 93;
            // 
            // chkStartWithWindows
            // 
            this.chkStartWithWindows.AutoSize = true;
            this.chkStartWithWindows.Location = new System.Drawing.Point(2, 2);
            this.chkStartWithWindows.Margin = new System.Windows.Forms.Padding(2);
            this.chkStartWithWindows.Name = "chkStartWithWindows";
            this.chkStartWithWindows.Size = new System.Drawing.Size(117, 17);
            this.chkStartWithWindows.TabIndex = 7;
            this.chkStartWithWindows.Text = "Start with Windows";
            this.chkStartWithWindows.UseVisualStyleBackColor = true;
            this.chkStartWithWindows.CheckedChanged += new System.EventHandler(this.chkStartWithWindows_CheckedChanged);
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMain.Controls.Add(this.lvStatus, 0, 1);
            this.tlpMain.Controls.Add(this.flpTopRight, 1, 0);
            this.tlpMain.Controls.Add(this.flpTopLeft, 0, 0);
            this.tlpMain.Controls.Add(this.lvStatistics, 1, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(2);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Size = new System.Drawing.Size(667, 281);
            this.tlpMain.TabIndex = 8;
            // 
            // flpTopRight
            // 
            this.flpTopRight.Controls.Add(this.chkStartWithWindows);
            this.flpTopRight.Controls.Add(this.btnStartStop);
            this.flpTopRight.Controls.Add(this.btnSettings);
            this.flpTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTopRight.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpTopRight.Location = new System.Drawing.Point(254, 2);
            this.flpTopRight.Margin = new System.Windows.Forms.Padding(2);
            this.flpTopRight.Name = "flpTopRight";
            this.flpTopRight.Size = new System.Drawing.Size(411, 136);
            this.flpTopRight.TabIndex = 7;
            // 
            // flpTopLeft
            // 
            this.flpTopLeft.Controls.Add(this.gbBotMode);
            this.flpTopLeft.Controls.Add(this.gbCoinPair);
            this.flpTopLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTopLeft.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpTopLeft.Location = new System.Drawing.Point(2, 2);
            this.flpTopLeft.Margin = new System.Windows.Forms.Padding(2);
            this.flpTopLeft.Name = "flpTopLeft";
            this.flpTopLeft.Size = new System.Drawing.Size(248, 136);
            this.flpTopLeft.TabIndex = 8;
            // 
            // lvStatistics
            // 
            this.lvStatistics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chStatistic,
            this.chStatisticValue});
            this.lvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvStatistics.FullRowSelect = true;
            this.lvStatistics.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            this.lvStatistics.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.lvStatistics.Location = new System.Drawing.Point(255, 143);
            this.lvStatistics.Name = "lvStatistics";
            this.lvStatistics.Size = new System.Drawing.Size(409, 135);
            this.lvStatistics.TabIndex = 1;
            this.lvStatistics.UseCompatibleStateImageBehavior = false;
            this.lvStatistics.View = System.Windows.Forms.View.Details;
            // 
            // chStatistic
            // 
            this.chStatistic.Text = "Statistic";
            this.chStatistic.Width = 230;
            // 
            // chStatisticValue
            // 
            this.chStatisticValue.Text = "Value";
            this.chStatisticValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chCost
            // 
            this.chCost.Text = "Cost ($)";
            this.chCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 421);
            this.Controls.Add(this.tlpMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.gbBotMode.ResumeLayout(false);
            this.gbCoinPair.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            this.flpTopRight.ResumeLayout(false);
            this.flpTopRight.PerformLayout();
            this.flpTopLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.ComboBox cboBotMode;
        private System.Windows.Forms.GroupBox gbBotMode;
        private System.Windows.Forms.GroupBox gbCoinPair;
        private System.Windows.Forms.ComboBox cboCoinPairDefaultNew;
        private MyListView lvStatus;
        private System.Windows.Forms.ColumnHeader chID;
        private System.Windows.Forms.ColumnHeader chCoinPair;
        private System.Windows.Forms.ColumnHeader chBuyPrice;
        private System.Windows.Forms.ColumnHeader chMarketPrice;
        private System.Windows.Forms.ColumnHeader chPriceChangePerc;
        private System.Windows.Forms.CheckBox chkStartWithWindows;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.FlowLayoutPanel flpTopRight;
        private System.Windows.Forms.FlowLayoutPanel flpTopLeft;
        private System.Windows.Forms.ColumnHeader chQuantity;
        private MyListView lvStatistics;
        private System.Windows.Forms.ColumnHeader chStatistic;
        private System.Windows.Forms.ColumnHeader chStatisticValue;
        private System.Windows.Forms.ColumnHeader chCost;
    }
}

