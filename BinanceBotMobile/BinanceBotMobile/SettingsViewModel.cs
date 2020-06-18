using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace BinanceBot
{
    public class SettingsViewModel : BaseViewModel
    {
        public override string APIKey
        {
            get => Preferences.Get(nameof(APIKey), "");
            set { SetPreference(nameof(APIKey), value); }
        }

        public override string SecretKey
        {
            get => Preferences.Get(nameof(SecretKey), "");
            set { SetPreference(nameof(SecretKey), value); }
        }

        // Futures

        public override double FuturesSafetyFactor
        {
            get => Preferences.Get(nameof(FuturesSafetyFactor), 15.0);
            set { SetPreference(nameof(FuturesSafetyFactor), value); }
        }

        public override bool IsAutoAdjustShortAboveAndLongBelow
        {
            get => Preferences.Get(nameof(IsAutoAdjustShortAboveAndLongBelow), false);
            set { SetPreference(nameof(IsAutoAdjustShortAboveAndLongBelow), value); }
        }

        public override double ShortAbove
        {
            get => Preferences.Get(nameof(ShortAbove), 9990.0);
            set { SetPreference(nameof(ShortAbove), value); }
        }

        public override double LongBelow
        {
            get => Preferences.Get(nameof(LongBelow), 9490.0);
            set { SetPreference(nameof(LongBelow), value); }
        }

        public override bool IsAutoAdjustTargetProfit
        {
            get => Preferences.Get(nameof(IsAutoAdjustTargetProfit), false);
            set { SetPreference(nameof(IsAutoAdjustTargetProfit), value); }
        }

        public override double FuturesProfitTarget
        {
            get => Preferences.Get(nameof(FuturesProfitTarget), 0.0);
            set { SetPreference(nameof(FuturesProfitTarget), value); }
        }
    }
}