using Binance.Net.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExchangeClientLib
{
    public partial class TradingData
    {
        public ListViewItem ToListViewItem()
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = ID.ToString();

            if (CoinQuantity == 0)
                lvi.SubItems.Add(CoinOriginalQuantity.ToString());
            else
                lvi.SubItems.Add(CoinQuantity.ToString());

            lvi.SubItems.Add(CoinPair.ToString());
            lvi.SubItems.Add(LastAction.ToString());

            if (LastAction == OrderSide.Buy)
                lvi.SubItems.Add(BuyPriceAfterFees.ToString());
            else
                lvi.SubItems.Add(SellPriceAfterFees.ToString());

            lvi.SubItems.Add(Cost.ToString());
            lvi.SubItems.Add(Price.ToString());
            lvi.SubItems.Add(PriceChangePercentage.ToString());
            lvi.ForeColor = PriceChangePercentage > 0m ? Color.Green : Color.Red;
            return lvi;
        }
    }
}