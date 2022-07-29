// -------------------------------------------------------------------------------------------------
//
//    This code is a cTrader Automate API example.
//
//    This cBot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk.
//    
//    All changes to this file might be lost on the next application update.
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
//    The "Sample Trend cBot" will buy when fast period moving average crosses the slow period moving average and sell when 
//    the fast period moving average crosses the slow period moving average. The orders are closed when an opposite signal 
//    is generated. There can only by one Buy or Sell order at any time.
//
// -------------------------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class HiloActivator : Robot
    {
        [Parameter("Percentage of Account (%)", Group = "Volume", DefaultValue = 100, MinValue = 1, Step = 1)]
        public double Percentage { get; set; }

        [Parameter("MA Type", Group = "Moving Average")]
        public MovingAverageType MAType { get; set; }

        [Parameter("High", Group = "Moving Average", DefaultValue = "High")]
        public DataSeries High {get; set;}
        
        [Parameter("Low", Group = "Moving Average", DefaultValue = "Low")]
        public DataSeries Low {get; set;}

        [Parameter("Periods", Group = "Moving Average", DefaultValue = 6)]
        public int Periods { get; set; }

        private MovingAverage highMa;
        private MovingAverage lowMa;
        private const string label = "Sample Trend cBot";

        protected override void OnStart()
        {
            highMa = Indicators.MovingAverage(High, Periods, MAType);
            lowMa = Indicators.MovingAverage(Low, Periods, MAType);
        }

        protected override void OnBar()
        {
            var longPosition = Positions.Find(label, SymbolName, TradeType.Buy);
            var shortPosition = Positions.Find(label, SymbolName, TradeType.Sell);

            var currentlowMa = lowMa.Result.Last(1);
            var currentHighMa = highMa.Result.Last(1);
            var previouslowMa = lowMa.Result.Last(3);
            var previousHighMa = highMa.Result.Last(3);

            if (currentHighMa <= Bars.ClosePrices.Last(1)  && longPosition == null)
            {
                if (shortPosition != null)
                    ClosePosition(shortPosition);

                ExecuteMarketOrder(TradeType.Buy, SymbolName, VolumeInUnits, label);
            }
            else if (currentlowMa > Bars.ClosePrices.Last(1) && shortPosition == null)
            {
                if (longPosition != null)
                    ClosePosition(longPosition);

               //ExecuteMarketOrder(TradeType.Sell, SymbolName, VolumeInUnits, label);
            }
        }

        private double VolumeInUnits
        {
            get { return (((int)((Account.Balance / Symbol.Ask) * (Percentage / 100)) * 100) / 100); }
        }
    }
}
