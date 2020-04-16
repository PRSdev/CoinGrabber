using Binance.Net.Objects;
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

        private decimal _marketPrice;
        public decimal MarketPrice
        {
            get
            {
                return _marketPrice;
            }
            set
            {
                _marketPrice = Math.Round(value, 2);
            }
        }

        public CoinPair CoinPair { get; set; }

        private decimal _capitalCost;
        public decimal CapitalCost
        {
            get
            {
                return _capitalCost;
            }
            set
            {
                _capitalCost = Math.Round(value, 2);
            }
        }

        private decimal _quantity;
        public decimal CoinQuantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = Math.Round(value, 5);

                if (CoinOriginalQuantity == 0)
                    CoinOriginalQuantity = _quantity;
            }
        }

        public decimal CoinOriginalQuantity { get; set; }

        private decimal _priceChangePerc;
        public decimal PriceChangePercentage
        {
            get
            {
                return _priceChangePerc;
            }
            set
            {
                _priceChangePerc = Math.Round(_priceChangePerc, 2);
            }
        }

        private decimal _buyPrice;
        public decimal BuyPriceAfterFees
        {
            get
            {
                return _buyPrice;
            }
            set
            {
                _buyPrice = Math.Round(value, 2);
            }
        }

        private decimal _sellPrice;
        public decimal SellPriceAfterFees
        {
            get
            {
                return _sellPrice;
            }
            set
            {
                _sellPrice = Math.Round(value, 2);
            }
        }
        public long BuyOrderID { get; set; } = -1;
        public long SellOrderID { get; set; } = -1;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public OrderSide LastAction { get; set; }

        public decimal Profit
        {
            get
            {
                return SellPriceAfterFees == 0 ? 0 : (SellPriceAfterFees - BuyPriceAfterFees) * CoinQuantity;
            }
        }

        public TradingData()
        {
        }

        public TradingData(CoinPair coinPair)
        {
            CoinPair = coinPair;
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
            StringBuilder sb = new StringBuilder();
            sb.Append($"ID={ID} Sold {CoinQuantity} {CoinPair.Pair1} for {MarketPrice}");
            if (BuyPriceAfterFees > 0)
                sb.Append($" with profit {Profit}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return LastAction == OrderSide.Sell ? ToStringSold() : ToStringBought();
        }

        public ListViewItem ToListViewItem()
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = ID.ToString();
            lvi.SubItems.Add(CoinQuantity.ToString());
            lvi.SubItems.Add(CoinPair.ToString());
            lvi.SubItems.Add(BuyPriceAfterFees.ToString());
            lvi.SubItems.Add(CapitalCost.ToString());
            lvi.SubItems.Add(MarketPrice.ToString());
            lvi.SubItems.Add(PriceChangePercentage.ToString());
            lvi.ForeColor = PriceChangePercentage > 0m ? Color.Green : Color.Red;
            return lvi;
        }
    }
}