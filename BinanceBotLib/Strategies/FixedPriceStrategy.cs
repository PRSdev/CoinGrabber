using ExchangeClientLib;

namespace BinanceBotLib
{
    public class FixedPriceStrategy : Strategy
    {
        public FixedPriceStrategy(ExchangeType exchange, Settings settings) : base(exchange, settings)
        {
        }

        public override void Trade()
        {
            PlaceBuyOrder();
        }

        public override void PlaceBuyOrder()
        {
            decimal coinUSD = _client.GetBalance(_settings.CoinPair.Pair2);
            Bot.WriteConsole($"{_settings.CoinPair.Pair1} balance: {coinUSD}");

            TradingData tradingData = new TradingData();
            tradingData.CoinPair = _settings.CoinPair;
            _client.PlaceTestBuyOrder(tradingData);
        }

        public override void PlaceSellOrder()
        {

        }
    }
}