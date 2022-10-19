using ExchangeClientLib;
using ShareX.HelpersLib;
using System;
using System.Text;
using System.Timers;

namespace BinanceBotLib
{
    public class Bot
    {
        private static ExchangeType _exchangeType = ExchangeType.BinanceExchange;
        private System.Timers.Timer _marketTimer = new System.Timers.Timer();
        public Strategy Strategy { get; private set; }

        #region IO

        public Settings Settings { get; set; }

        private static Logger logger = new Logger(SettingsManager.LogFilePath);

        public static void WriteConsole(string message = "")
        {
            if (_exchangeType != ExchangeType.MockupExchange)
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteLog(string message)
        {
            if (_exchangeType != ExchangeType.MockupExchange)
            {
                Console.WriteLine(message);
                logger.WriteLine(message);
            }
        }

        #endregion IO

        public Bot(Settings settings = null)
        {
            if (settings == null)
                Settings = SettingsManager.LoadSettings();
            else
                Settings = settings;

            if (!string.IsNullOrEmpty(Settings.APIKey))
                Init();
        }

        public static ExchangeType GetExchangeType()
        {
            return _exchangeType;
        }

        private void Init()
        {
            double timerInterval = 100; // milliseconds

            switch (Settings.BotMode)
            {
                case BotMode.FixedPrice:
                    _marketTimer.Interval = timerInterval;
                    Strategy = new FixedPriceStrategy(_exchangeType, Settings);
                    break;

                default:
                    Console.WriteLine("Unhandled Bot Mode.");
                    Console.ReadLine();
                    return;
            }
        }


        public void Start()
        {
            Init();

#if DEBUG
            Strategy.Activate();
#endif
            _marketTimer.Elapsed += MarketTimer_Tick;
            _marketTimer.Start();
        }

        public void Stop()
        {
            _marketTimer.Stop();
        }

        public string ToStatusString(TradingData data)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Profit mode: {Settings.TakeProfitMode.ToString()}");
            sb.AppendLine($"Market Price: {data.Price}");
            sb.AppendLine($"Entry Price: {data.BuyPriceAfterFees}");
            if (data.PriceLongBelow > 0)
            {
                sb.AppendLine($"Long Below: {data.PriceLongBelow}");
                sb.AppendLine($"Short Above: {data.PriceShortAbove}");
                sb.AppendLine($"Target Profit: {data.ProfitTarget}");
            }

            return sb.ToString();
        }

        private void MarketTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (_exchangeType != ExchangeType.MockupExchange && string.IsNullOrEmpty(Settings.APIKey))
                throw new Exception("Settings reset!");

            try
            {
                Strategy.Activate();
            }
            catch (ArgumentOutOfRangeException) // Mockup Exchange Client
            {
                if (_exchangeType == ExchangeType.MockupExchange)
                {
                    string result = $"{Settings.HydraFactor},{Settings.PriceChangePercentageDown},{Settings.PriceChangePercentageUp},{Strategy.Statistics.GetPortfolioValue()}{Strategy.Statistics.GetCoinsBalanceCsv()}";
                    Console.WriteLine(result);
                    Stop();
                }
            }
            catch (Exception ex)
            {
                Bot.WriteLog(ex.Message);
            }
        }
    }
}