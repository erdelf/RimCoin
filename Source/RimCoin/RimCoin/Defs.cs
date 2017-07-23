using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace RimCoin
{
    public abstract class PCPartDef : ThingDef
    {
        public int spaceCost = 0;
        public int powerDraw = 0;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
             this.thingClass = typeof(PCPart);
        }

        public virtual string InspectString => "SpaceCostInspect".Translate(this.spaceCost) + Environment.NewLine + "PowerDrawInspect".Translate(this.powerDraw);
    }
    
    public abstract class PCSlotPartDef : PCPartDef
    {
        public string slot;

        public override string InspectString => base.InspectString + Environment.NewLine + "NeededSlotInspect".Translate(this.slot);
    }

    public class PCMiningDef : PCSlotPartDef
    {
        public float miningFactor;

        public override string InspectString => base.InspectString + Environment.NewLine + "MiningFactorInspect".Translate(this.miningFactor);
    }

    public class PCMotherboardDef : PCPartDef
    {
        public List<PCSlotCountEntry> slots;

        public override string InspectString => base.InspectString;
    }

    public class PCCoolerDef : PCSlotPartDef
    {
        public float heatDisplacement;

        public override string InspectString => base.InspectString + Environment.NewLine + "HeatDisplacementInspect".Translate(this.heatDisplacement);
    }

    public class PCCaseDef : ThingDef
    {
        public float caseSpace;
    }

    public class PCSlotCountEntry
    {
        public string slot;
        public int count;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            this.slot = xmlRoot.Name;
            this.count = (int)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(int));
        }
    }
}