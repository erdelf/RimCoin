﻿using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace RimCoin
{
    public class Building_Computer : Building
    {
        public List<PCPart> parts;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.parts = new List<PCPart>();
        }

        public Thing Motherboard => this.parts.FirstOrDefault(p => p.def is PCMotherboardDef);

        public PCCaseDef CaseDef => this.def as PCCaseDef;

        public virtual float UsedSpace => this.parts.Sum(p => (p.def as PCPartDef).spaceCost);

        public virtual float FreeSpace => this.CaseDef.caseSpace - this.UsedSpace;

        public override void TickLong()
        {
            if (this.GetComp<CompPowerTrader>().PowerOn)
            {
                if (this.AmbientTemperature > 80f)
                {
                    this.TryAttachFire(2f);
                } else
                {
                    Find.World.GetComponent<WorldComp_RimCoin>().RimCoinAmount += Mathf.RoundToInt(this.parts.Select(t => t.def).OfType<PCMiningDef>().Sum(pcd => 1 * pcd.miningFactor));
                    GenTemperature.PushHeat(this, Mathf.Abs(this.GetComp<CompPowerTrader>().PowerOutput));
                    UpdatePowerDraw();
                }
            }
        }

        public virtual bool HasFreeSlot(string slot) =>
            FreeSlotCount(slot) > 0;

        public virtual int FreeSlotCount(string slot) =>
            ((this.Motherboard?.def as PCMotherboardDef)?.slots.FirstOrDefault(psce => psce.slot.EqualsIgnoreCase(slot))?.count ?? 0) -
            this.parts.Count(t => (t.def as PCSlotPartDef)?.slot.EqualsIgnoreCase(slot) ?? false);

        public virtual bool AcceptsPart(Thing part) =>
            part?.def is PCPartDef pcp && pcp.spaceCost < this.FreeSpace && ((pcp is PCSlotPartDef pcsp && HasFreeSlot(pcsp.slot)) || (pcp is PCMotherboardDef && this.Motherboard == null));

        public virtual bool TryInstallPart(PCPart part)
        {
            if (AcceptsPart(part))
            {
                if(part.Spawned)
                    part.DeSpawn();
                this.parts.Add(part);
                UpdatePowerDraw();
                return true;
            }
            return false;
        }


        public virtual float HeatEnergy => Mathf.Abs(this.GetComp<CompPowerTrader>().PowerOutput/100) * this.parts.Select(t => t.def).OfType<PCCoolerDef>().Select(pcc => pcc.heatDisplacement).Aggregate((a,b) => a*b+a);

        public virtual bool TryRemovePart(PCPart part, IntVec3 loc)
        {
            if(this.parts.Remove(part))
            {
                GenSpawn.Spawn(part, loc, this.Map);
                UpdatePowerDraw();
                return true;
            }
            return false;
        }

        public virtual void UpdatePowerDraw() => 
            this.GetComp<CompPowerTrader>().PowerOutput = -this.parts.Sum(t => (t.def as PCPartDef).powerDraw);

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption fmo in base.GetFloatMenuOptions(selPawn))
                yield return fmo;
            if (!this.Map.reservationManager.IsReservedByAnyoneWhoseReservationsRespects(this, selPawn))
            {
                foreach (Thing thing in this.parts)
                    if (thing != this.Motherboard || this.parts.Count == 1)
                        yield return new FloatMenuOption("RCUninstallPCPartFloatMenu".Translate(thing.LabelCap), () =>
                            selPawn.jobs.TryTakeOrderedJob(new Job(RCDefOf.UninstallPCPart, this, thing)));

                yield return new FloatMenuOption("RCInstallPCPartFloatMenu".Translate(),
                    () => Find.Targeter.BeginTargeting(new TargetingParameters()
                    {
                        validator = lti => AcceptsPart(lti.Cell.GetFirstItem(lti.Map)),
                        canTargetItems = true,
                        canTargetLocations = true
                    }, lti => selPawn.jobs.TryTakeOrderedJob(new Job(RCDefOf.InstallPCPart, lti.Cell.GetFirstItem(this.Map), this))));
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
                yield return g;
            yield return new Command_Action()
            {
                defaultLabel = "",
                defaultDesc = "",
                disabled = !this.GetComp<CompPowerTrader>().PowerOn,
                disabledReason = "CannotUseNoPower".Translate(),
                action = () => WebPageUtility.OpenDialog()
            };
        }

        public override string GetInspectString() => 
            base.GetInspectString().Trim() + (this.holdingOwner.Owner as MinifiedThing != null ? "" : "\n" + "RCRimCoinInspect".Translate(Find.World.GetComponent<WorldComp_RimCoin>().RimCoinAmount.ToString("F3")));

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref this.parts, "pc parts", LookMode.Deep);
        }
    }
}