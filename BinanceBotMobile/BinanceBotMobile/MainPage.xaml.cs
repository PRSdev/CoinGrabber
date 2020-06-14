using BinanceBotLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BinanceBotMobile
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

        }

        public IList<string> BotModesList
        {
            get
            {
                List<string> modes = new List<string>();
                foreach (BotMode botMode in Helpers.GetEnums<BotMode>())
                {
                    modes.Add(botMode.GetDescription());
                }
                Console.WriteLine(modes.Count);
                return modes;
            }
        }

        private void btnSettings_Clicked(object sender, EventArgs e)
        {
          
        }
    }
}