using System;
using System.Collections.Generic;
using Verse;

namespace RimCoin
{
    public class PCPart : ThingWithComps
    {
        public PCPartDef PCPartDef => this.def as PCPartDef;

        public override string GetInspectString() => base.GetInspectString() + Environment.NewLine + this.PCPartDef.InspectString;
    }
}