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
//    The "Sample RSI cBot" will create a buy order when the Relative Strength Index indicator crosses the  level 30, 
//    and a Sell order when the RSI indicator crosses the level 70. The order is closed be either a Stop Loss, defined in 
//    the "Stop Loss" parameter, or by the opposite RSI crossing signal (buy orders close when RSI crosses the 70 level 
//    and sell orders are closed when RSI crosses the 30 level). 
//
//    The cBot can generate only one Buy or Sell order at any given time.
//
// -------------------------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;
namespace cAlgo
{
    // This sample cBot shows how to use the Supertrend indicator
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SupertrendSample : Robot
    {
        private Supertrend _supertrend;
        [Parameter("Percentage of Account (%)", Group = "Volume", DefaultValue = 100, MinValue = 1, Step = 1)]
        public double Percentage { get; set; }
        [Parameter("Label", DefaultValue = "SupertrendBot")]
        public string Label { get; set; }
        public Position[] BotPositions
        {
            get
            {
                return Positions.FindAll(Label);
            }
        }
        protected override void OnStart()
        {
            _supertrend = Indicators.Supertrend(10, 3);
        }
        protected override void OnBar()
        {
            if (_supertrend.UpTrend.Last(1) < Bars.ClosePrices.Last(1) && _supertrend.DownTrend.Last(2) > Bars.ClosePrices.Last(2))
            {
                ExecuteMarketOrder(TradeType.Buy, SymbolName, VolumeInUnits, Label);
            }
            else if (_supertrend.DownTrend.Last(1) > Bars.ClosePrices.Last(1) && _supertrend.UpTrend.Last(2) < Bars.ClosePrices.Last(2))
            {
                ClosePositions(TradeType.Buy);
            }
        }
        private void ClosePositions(TradeType tradeType)
        {
            foreach (var position in BotPositions)
            {
                if (position.TradeType != tradeType) continue;
                ClosePosition(position);
            }
        }
        
        private double VolumeInUnits
        {
            get { return (((int)((Account.Balance / Symbol.Ask) * 100)) / 100) * (Percentage / 100); }
        }
    }
}