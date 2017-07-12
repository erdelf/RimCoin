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
            this.pawn.CurJob.count = 1;
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, 1);
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, 1);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnForbidden(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            Toil toil = Toils_General.Wait(500);
            toil.WithProgressBarToilDelay(TargetIndex.B);
            yield return toil;
            yield return new Toil()
            {
                defaultDuration = 1,
                defaultCompleteMode = ToilCompleteMode.FinishedBusy,
                finishActions = new List<Action>(1) {
                    () => pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out Thing result, (t, i) => (TargetB.Thing as Building_Computer).TryInstallPart(t as PCPart))
                }
            };
        }
    }
}