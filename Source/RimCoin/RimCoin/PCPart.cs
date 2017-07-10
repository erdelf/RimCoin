using System;
using System.Collections.Generic;
using Verse;

namespace RimCoin
{
    public class PCPart : ThingWithComps
    {
        public PCPartDef PCPartDef => this.def as PCPartDef;

        public override string GetInspectString()
        {
            List<string> inspects = new List<string>
            {
                base.GetInspectString().Trim(),
                "SpaceCostInspect".Translate(this.PCPartDef.spaceCost)
            };

            return string.Join(Environment.NewLine, inspects.ToArray());
        }
    }
}
