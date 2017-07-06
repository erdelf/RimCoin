using RimWorld.Planet;
using Verse;

namespace RimCoin
{
    class WorldComp_RimCoin : WorldComponent
    {
        int rimCoinAmount;

        public int RimCoinAmount {
            get => this.rimCoinAmount;
            set => this.rimCoinAmount = value;
        }

        public WorldComp_RimCoin(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.rimCoinAmount, "rimCoins");
        }
    }
}