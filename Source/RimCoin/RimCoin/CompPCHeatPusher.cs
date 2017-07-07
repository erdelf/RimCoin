using RimWorld;
using Verse;

namespace RimCoin
{
    class CompPCHeatPusher : ThingComp
    {
        private const int HeatPushInterval = 60;

        public CompProperties_HeatPusher Props => (CompProperties_HeatPusher)this.props;

        protected virtual bool ShouldPushHeatNow => this.parent.GetComp<CompPowerTrader>().PowerOn;

        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.IsHashIntervalTick(60) && this.ShouldPushHeatNow)
            {
                CompProperties_HeatPusher props = this.Props;
                float ambientTemperature = this.parent.AmbientTemperature;
                GenTemperature.PushHeat(this.parent, (this.parent as Building_Computer).HeatEnergy);
            }
        }
    }
}