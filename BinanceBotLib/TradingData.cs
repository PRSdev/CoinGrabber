using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BinanceBotLib
{
    public class TradingData
    {
        public int ID { get; set; }
        public decimal MarketPrice { get; set; }
        public CoinPair CoinPair { get; set; }
        public decimal CapitalCost { get; set; }
        public decimal CoinQuantity { get; set; }
        public decimal PriceChangePercentage { get; set; }
        public decimal BuyPriceAfterFees { get; set; }
        public decimal SellPriceAfterFees { get; set; }

        public long BuyOrderID { get; set; } = -1;
        public long SellOrderID { get; set; } = -1;

        public decimal Profit
        {
            get
            {
                return SellPriceAfterFees == 0 ? 0 : Math.Round((SellPriceAfterFees - BuyPriceAfterFees) * CoinQuantity, 2);
            }
        }

        public static TradingData GetNew()
        {
            return new TradingData() { CoinPair = Bot.Settings.CoinPair };
        }

        public string ToStringPriceCheck()
        {
            return $"ID={ID} CoinPair={CoinPair.ToString()} BuyPriceAfterFees={BuyPriceAfterFees} MarketPrice={MarketPrice} Change={PriceChangePercentage}%";
        }

        public string ToStringBought()
        {
            return $"ID={ID} Bought {CoinQuantity} {CoinPair.Pair1} using {CapitalCost} for {MarketPrice}";
        }

        public string ToStringSold()
        {
            return $"ID={ID} Sold {CoinQuantity} {CoinPair.Pair1} for {MarketPrice} with profit {Profit}";
        }

        public override string ToString()
        {
            return Profit > 0 ? ToStringSold() : ToStringBought();
        }

        public ListViewItem ToListViewItem()
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = ID.ToString();
            lvi.SubItems.Add(CoinQuantity.ToString());
            lvi.SubItems.Add(CoinPair.ToString());
            lvi.SubItems.Add(BuyPriceAfterFees.ToString());
            lvi.SubItems.Add(MarketPrice.ToString());
            lvi.SubItems.Add(PriceChangePercentage.ToString());
            lvi.ForeColor = PriceChangePercentage > 0m ? Color.Green : Color.Red;
            return lvi;
        }
    }
}