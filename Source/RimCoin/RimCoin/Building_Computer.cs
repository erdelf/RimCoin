using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
    
namespace RimCoin
{
    public class Building_Computer : Building
    {
        public int bitCoinAmount;
        public List<Thing> parts;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.parts = new List<Thing>();
            this.bitCoinAmount = 0;
        }

        public Thing Motherboard => this.parts.FirstOrDefault(p => p.def is PCMotherboardDef);

        public PCCaseDef CaseDef => this.def as PCCaseDef;

        public float UsedSpace => this.parts.Sum(p => (p.def as PCPartDef).spaceCost);

        public float FreeSpace => this.CaseDef.caseSpace - this.UsedSpace;

        public override void TickLong()
        {
            if (this.GetComp<CompPowerTrader>().PowerOn)
            {
                this.bitCoinAmount += Mathf.RoundToInt(this.parts.Select(t => t.def).OfType<PCMiningDef>().Sum(pcd => 1 * pcd.miningFactor));
                this.GetComp<CompPowerTrader>().PowerOutput = -this.parts.Sum(t => (t.def as PCPartDef).powerDraw);
            }
        }

        public bool HasFreeSlot(string slot) => 
            ((this.Motherboard?.def as PCMotherboardDef)?.slots.FirstOrDefault(psce => psce.slot.EqualsIgnoreCase(slot))?.count ?? 0) > 
            this.parts.Count(t => (t.def as PCSlotPart)?.slot.EqualsIgnoreCase(slot) ?? false);

        public bool AcceptsPart(Thing part) =>
            part?.def is PCPartDef pcp && pcp.spaceCost < this.FreeSpace && ((pcp is PCSlotPart pcsp && HasFreeSlot(pcsp.slot)) || (pcp is PCMotherboardDef && this.Motherboard == null));

        public bool TryInstallPart(Thing part)
        {
            if (AcceptsPart(part))
            {
                if(part.Spawned)
                    part.DeSpawn();
                this.parts.Add(part);
                return true;
            }
            return false;
        }

        public bool TryRemovePart(Thing part) => this.parts.Remove(part);

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption fmo in base.GetFloatMenuOptions(selPawn))
                yield return fmo;

            foreach (Thing thing in this.parts)
                if (thing != this.Motherboard || this.parts.Count == 1)
                    yield return new FloatMenuOption("uninstall " + thing.LabelCap, () => TryRemovePart(thing));

            yield return new FloatMenuOption("install new part",
                () => Find.Targeter.BeginTargeting(new TargetingParameters()
                {
                    validator = lti => AcceptsPart(lti.Cell.GetFirstItem(lti.Map)),
                    canTargetItems = true,
                    canTargetLocations = true
                }, lti => selPawn.jobs.TryTakeOrderedJob(new Job(RCDefOf.InstallPCPart, lti.Cell.GetFirstItem(this.Map), this))));
        }

        public override string GetInspectString() => 
            base.GetInspectString().Trim() + "\nBitCoins: " + this.bitCoinAmount;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.bitCoinAmount, "bitCoins");
            Scribe_Collections.Look(ref this.parts, "pc parts", LookMode.Deep);
        }
    }
}
