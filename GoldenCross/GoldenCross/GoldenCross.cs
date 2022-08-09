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
    public class GoldenCross : Robot
    {
        [Parameter("Label", DefaultValue = "GoldenCrossBot")]
        public string Label { get; set; }
    
        [Parameter("Percentage of Account (%)", Group = "Volume", DefaultValue = 100, MinValue = 1, Step = 1)]
        public double Percentage { get; set; }

        [Parameter("MA Type", Group = "Moving Average")]
        public MovingAverageType MAType { get; set; }

        [Parameter("Source", Group = "Moving Average")]
        public DataSeries SourceSeries { get; set; }

        [Parameter("Slow Periods", Group = "Moving Average", DefaultValue = 48)]
        public int SlowPeriods { get; set; }

        [Parameter("Fast Periods", Group = "Moving Average", DefaultValue = 13)]
        public int FastPeriods { get; set; }


        private MovingAverage slowMa;
        private MovingAverage fastMa;
        
        protected override void OnStart()
        {
            fastMa = Indicators.MovingAverage(SourceSeries, FastPeriods, MAType);
            slowMa = Indicators.MovingAverage(SourceSeries, SlowPeriods, MAType);
        }

        protected override void OnBar()
        {
            var longPosition = Positions.Find(Label, SymbolName, TradeType.Buy);
            var shortPosition = Positions.Find(Label, SymbolName, TradeType.Sell);

            var currentSlowMa = slowMa.Result.Last(1);
            var currentFastMa = fastMa.Result.Last(1);
            var previousSlowMa = slowMa.Result.Last(2);
            var previousFastMa = fastMa.Result.Last(2);

            if (previousSlowMa > previousFastMa && currentSlowMa <= currentFastMa && longPosition == null)
            {
                if (shortPosition != null)
                    ClosePosition(shortPosition);

                ExecuteMarketOrder(TradeType.Buy, SymbolName, VolumeInUnits, Label);
            }
            else if (previousSlowMa < previousFastMa && currentSlowMa >= currentFastMa && shortPosition == null)
            {
                if (longPosition != null)
                    ClosePosition(longPosition);

               ExecuteMarketOrder(TradeType.Sell, SymbolName, VolumeInUnits, Label);
            }
        }

        private double VolumeInUnits
        {
            get { return (((int)((Account.Balance / Symbol.Ask) * (Percentage / 100)) * 100) / 100); }
        }
    }
}
