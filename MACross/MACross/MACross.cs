using cAlgo.API;
using cAlgo.API.Indicators;
namespace cAlgo
{
    // This sample cBot shows how to use the Supertrend indicator
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MACross : Robot
    {
        [Parameter("Label", DefaultValue = "MACross")]
        public string Label { get; set; }
    
        [Parameter("Percentage of Account (%)", Group = "Volume", DefaultValue = 100, MinValue = 1, Step = 1)]
        public double Percentage { get; set; }

        [Parameter("MA Type", Group = "Moving Average")]
        public MovingAverageType MAType { get; set; }

        [Parameter("Source", Group = "Moving Average")]
        public DataSeries SourceSeries { get; set; }

        [Parameter("Periods", Group = "Moving Average", DefaultValue = 50)]
        public int Periods { get; set; }
        
        private MovingAverage _ma;
        protected override void OnStart()
        {
            _ma = Indicators.MovingAverage(SourceSeries, Periods, MAType);
        }
        
        protected override void OnBar()
        {
            var longPosition = Positions.Find(Label, SymbolName, TradeType.Buy);
            
            var currentMa = _ma.Result.Last(1);
            var previousMa = _ma.Result.Last(2);
        
            if (currentMa < Bars.ClosePrices.Last(1) && previousMa > Bars.ClosePrices.Last(2))
                ExecuteMarketOrder(TradeType.Buy, SymbolName, VolumeInUnits, Label);
            else if (currentMa > Bars.ClosePrices.Last(1) && previousMa < Bars.ClosePrices.Last(2))
                ClosePosition(longPosition);
        }
        
        private double VolumeInUnits
        {
            get { return (((int)((Account.Balance / Symbol.Ask) * 100)) / 100) * (Percentage / 100); }
        }
    }
}