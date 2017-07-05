using System.Collections.Generic;
using System.Xml;
using Verse;

namespace RimCoin
{
    public class PCPartDef : ThingDef
    {
        public float spaceCost;
        public float powerDraw = 0;
    }
    
    public class PCSlotPart : PCPartDef
    {
        public string slot;
    }

    public class PCMiningDef : PCSlotPart
    {
        public float miningFactor;
    }

    public class PCMotherboardDef : PCPartDef
    {
        public List<PCSlotCountEntry> slots;
    }

    public class PCCoolerDef : PCSlotPart
    {
        public float heatDisplacement;
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