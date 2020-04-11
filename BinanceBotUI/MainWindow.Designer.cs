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
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.gbStatistics = new System.Windows.Forms.GroupBox();
            this.lblProfitTotal = new System.Windows.Forms.Label();
            this.cboBotMode = new System.Windows.Forms.ComboBox();
            this.gbBotMode = new System.Windows.Forms.GroupBox();
            this.gbCoinPair = new System.Windows.Forms.GroupBox();
            this.cboNewDefaultCoinPair = new System.Windows.Forms.ComboBox();
            this.lvStatus = new System.Windows.Forms.ListView();
            this.chID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCoinPair = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chBuyPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMarketPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPriceChangePerc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbStatistics.SuspendLayout();
            this.gbBotMode.SuspendLayout();
            this.gbCoinPair.SuspendLayout();
            this.SuspendLayout();
            // 
            // niTray
            // 
            this.niTray.Text = "notifyIcon1";
            this.niTray.Visible = true;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(16, 200);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(112, 48);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(136, 200);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(112, 48);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "Settings...";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // gbStatistics
            // 
            this.gbStatistics.Controls.Add(this.lblProfitTotal);
            this.gbStatistics.Location = new System.Drawing.Point(456, 16);
            this.gbStatistics.Name = "gbStatistics";
            this.gbStatistics.Size = new System.Drawing.Size(304, 248);
            this.gbStatistics.TabIndex = 2;
            this.gbStatistics.TabStop = false;
            this.gbStatistics.Text = "Statistics";
            // 
            // lblProfitTotal
            // 
            this.lblProfitTotal.AutoSize = true;
            this.lblProfitTotal.Location = new System.Drawing.Point(16, 32);
            this.lblProfitTotal.Name = "lblProfitTotal";
            this.lblProfitTotal.Size = new System.Drawing.Size(85, 20);
            this.lblProfitTotal.TabIndex = 0;
            this.lblProfitTotal.Text = "Total Profit";
            // 
            // cboBotMode
            // 
            this.cboBotMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBotMode.FormattingEnabled = true;
            this.cboBotMode.Location = new System.Drawing.Point(16, 32);
            this.cboBotMode.Name = "cboBotMode";
            this.cboBotMode.Size = new System.Drawing.Size(232, 28);
            this.cboBotMode.TabIndex = 3;
            this.cboBotMode.SelectedIndexChanged += new System.EventHandler(this.cboBotMode_SelectedIndexChanged);
            // 
            // gbBotMode
            // 
            this.gbBotMode.Controls.Add(this.cboBotMode);
            this.gbBotMode.Location = new System.Drawing.Point(8, 8);
            this.gbBotMode.Name = "gbBotMode";
            this.gbBotMode.Size = new System.Drawing.Size(296, 80);
            this.gbBotMode.TabIndex = 4;
            this.gbBotMode.TabStop = false;
            this.gbBotMode.Text = "Bot mode";
            // 
            // gbCoinPair
            // 
            this.gbCoinPair.Controls.Add(this.cboNewDefaultCoinPair);
            this.gbCoinPair.Location = new System.Drawing.Point(8, 104);
            this.gbCoinPair.Name = "gbCoinPair";
            this.gbCoinPair.Size = new System.Drawing.Size(296, 80);
            this.gbCoinPair.TabIndex = 5;
            this.gbCoinPair.TabStop = false;
            this.gbCoinPair.Text = "Coin Pair for new trades";
            // 
            // cboNewDefaultCoinPair
            // 
            this.cboNewDefaultCoinPair.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNewDefaultCoinPair.FormattingEnabled = true;
            this.cboNewDefaultCoinPair.Location = new System.Drawing.Point(16, 32);
            this.cboNewDefaultCoinPair.Name = "cboNewDefaultCoinPair";
            this.cboNewDefaultCoinPair.Size = new System.Drawing.Size(232, 28);
            this.cboNewDefaultCoinPair.TabIndex = 3;
            this.cboNewDefaultCoinPair.SelectedIndexChanged += new System.EventHandler(this.cboNewDefaultCoinPair_SelectedIndexChanged);
            // 
            // lvStatus
            // 
            this.lvStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.chID,
            this.chCoinPair,
            this.chBuyPrice,
            this.chMarketPrice,
            this.chPriceChangePerc});
            this.lvStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lvStatus.HideSelection = false;
            this.lvStatus.Location = new System.Drawing.Point(0, 280);
            this.lvStatus.Name = "lvStatus";
            this.lvStatus.Size = new System.Drawing.Size(778, 264);
            this.lvStatus.TabIndex = 6;
            this.lvStatus.UseCompatibleStateImageBehavior = false;
            this.lvStatus.View = System.Windows.Forms.View.Details;
            // 
            // chID
            // 
            this.chID.DisplayIndex = 0;
            this.chID.Text = "ID";
            this.chID.Width = 50;
            // 
            // chCoinPair
            // 
            this.chCoinPair.DisplayIndex = 1;
            this.chCoinPair.Text = "Coin Pair";
            this.chCoinPair.Width = 120;
            // 
            // chBuyPrice
            // 
            this.chBuyPrice.DisplayIndex = 2;
            this.chBuyPrice.Text = "Buy Price";
            this.chBuyPrice.Width = 100;
            // 
            // chMarketPrice
            // 
            this.chMarketPrice.DisplayIndex = 3;
            this.chMarketPrice.Text = "Market Price";
            this.chMarketPrice.Width = 100;
            // 
            // chPriceChangePerc
            // 
            this.chPriceChangePerc.DisplayIndex = 4;
            this.chPriceChangePerc.Text = "Price Change (%)";
            this.chPriceChangePerc.Width = 140;
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 5;
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 1;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 544);
            this.Controls.Add(this.lvStatus);
            this.Controls.Add(this.gbCoinPair);
            this.Controls.Add(this.gbBotMode);
            this.Controls.Add(this.gbStatistics);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnStartStop);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.gbStatistics.ResumeLayout(false);
            this.gbStatistics.PerformLayout();
            this.gbBotMode.ResumeLayout(false);
            this.gbCoinPair.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.GroupBox gbStatistics;
        private System.Windows.Forms.Label lblProfitTotal;
        private System.Windows.Forms.ComboBox cboBotMode;
        private System.Windows.Forms.GroupBox gbBotMode;
        private System.Windows.Forms.GroupBox gbCoinPair;
        private System.Windows.Forms.ComboBox cboNewDefaultCoinPair;
        private System.Windows.Forms.ListView lvStatus;
        private System.Windows.Forms.ColumnHeader chID;
        private System.Windows.Forms.ColumnHeader chCoinPair;
        private System.Windows.Forms.ColumnHeader chBuyPrice;
        private System.Windows.Forms.ColumnHeader chMarketPrice;
        private System.Windows.Forms.ColumnHeader chPriceChangePerc;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

