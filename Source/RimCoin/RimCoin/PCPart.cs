using System;
using Verse;

namespace RimCoin
{
    public class PCPart : ThingWithComps
    {
        public PCPartDef PCPartDef => this.def as PCPartDef;

        public override string GetInspectString() => (base.GetInspectString() + Environment.NewLine + this.PCPartDef.InspectString).Trim();
    }
}