using BinanceBotLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BinanceBotWinSvc
{
    public partial class BinanceBotWinSvc : ServiceBase
    {
        public BinanceBotWinSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Settings settings = Bot.LoadSettings();
            Bot myBot = new Bot(settings);
            myBot.Start(settings);
        }

        protected override void OnStop()
        {
        }
    }
}