﻿using Binance.Net.Enums;
using Newtonsoft.Json;
using System;
using System.Text;

namespace ExchangeClientLib
{
    public partial class TradingData
    {
        public int ID { get; set; }

        private decimal _marketPrice;
        [JsonIgnore]
        public decimal Price
        {
            get
            {
                return Math.Round(_marketPrice, 2);
            }
            private set
            {
                _marketPrice = value;
            }
        }

        public CoinPair CoinPair { get; set; }

        private decimal _quantityRemaining;
        public decimal CoinQuantity
        {
            get
            {
                return Math.Round(_quantityRemaining, CoinPair.Precision);
            }
            set
            {
                _quantityToTrade = Math.Abs(value);
                _quantityRemaining = _quantityToTrade;

                if (CoinOriginalQuantity == 0)
                    CoinOriginalQuantity = _quantityRemaining;
            }
        }

        private decimal _quantityToTrade;
        [JsonIgnore]
        public decimal CoinQuantityToTrade
        {
            get
            {
                return Math.Round(_quantityToTrade, 2);
            }
            set
            {
                _quantityToTrade = value;
                _quantityRemaining = _quantityRemaining - _quantityToTrade;
            }
        }

        public decimal CoinOriginalQuantity { get; private set; }

        private decimal _priceChangePerc;
        [JsonIgnore]
        public decimal PriceChangePercentage
        {
            get
            {
                return Math.Round(_priceChangePerc, 2);
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
                return SellPriceAfterFees == 0 ? 0 : (SellPriceAfterFees - BuyPriceAfterFees) * CoinQuantityToTrade;
            }
        }

        [JsonIgnore]
        public decimal Cost
        {
            get
            {
                decimal quantity = CoinQuantity == 0 ? CoinQuantityToTrade : CoinQuantity;
                decimal cost = LastAction == OrderSide.Buy ? BuyPriceAfterFees * quantity : SellPriceAfterFees * quantity;
                return Math.Round(cost, 2);
            }
        }

        [JsonIgnore]
        public double PriceLongBelow { get; set; }

        [JsonIgnore]
        public double PriceShortAbove { get; set; }

        [JsonIgnore]
        public double ProfitTarget { get; set; }

        public TradingData()
        {
        }

        public TradingData(CoinPair coinPair)
        {
            CoinPair = coinPair;
        }

        public bool UpdatePrice(decimal marketPrice)
        {
            if (marketPrice > 0)
            {
                Price = marketPrice;
            }

            return marketPrice > 0;
        }

        public void SetQuantity(decimal value)
        {
            CoinOriginalQuantity = value;
            CoinQuantity = value;
        }

        public void SetPriceChangePercentage(decimal marketPrice, bool isFutures = false)
        {
            if (SellPriceAfterFees > 0)
            {
                _priceChangePerc = (marketPrice - SellPriceAfterFees) / SellPriceAfterFees * 100;
                if (isFutures && LastAction == OrderSide.Sell)
                    _priceChangePerc = -_priceChangePerc;
            }
            else if (BuyPriceAfterFees > 0)
            {
                _priceChangePerc = (marketPrice - BuyPriceAfterFees) / BuyPriceAfterFees * 100;
            }
        }

        public string ToStringPriceCheck()
        {
            return $"ID={ID} CoinPair={CoinPair.ToString()} BuyPriceAfterFees={BuyPriceAfterFees} MarketPrice={Price} Change={PriceChangePercentage}%";
        }

        public string ToStringBought()
        {
            return $"ID={ID}; Side=Buy; Quantity={CoinQuantityToTrade}; Coin={CoinPair.Pair1}; Cost={Cost}; Price={Price}";
        }

        public string ToStringSold()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"ID={ID}; Side=Sell; Quantity={CoinQuantityToTrade}; Coin={CoinPair.Pair1}; Price={Price};");
            if (BuyPriceAfterFees > 0)
                sb.Append($" Profit={Profit}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return LastAction == OrderSide.Sell ? ToStringSold() : ToStringBought();
        }
    }
}