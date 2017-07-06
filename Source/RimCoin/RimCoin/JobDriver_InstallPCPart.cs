using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimCoin
{
    class JobDriver_InstallPCPart : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnForbidden(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return new Toil()
            {
                defaultDuration = 50,
                defaultCompleteMode = ToilCompleteMode.FinishedBusy,
                finishActions = new List<Action>(1) {
                    () => pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out Thing result, (t, i) => (TargetB.Thing as Building_Computer).TryInstallPart(t))
                }
            };
        }
    }
}