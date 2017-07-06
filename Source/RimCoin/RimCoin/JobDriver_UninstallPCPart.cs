using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;

namespace RimCoin
{
    class JobDriver_UninstallPCPart : JobDriver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, 1);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnForbidden(TargetIndex.A);
            Toil toil = Toils_General.Wait(500);
            toil.WithProgressBarToilDelay(TargetIndex.B);
            yield return toil;
            yield return new Toil()
            {
                defaultDuration = 1,
                defaultCompleteMode = ToilCompleteMode.FinishedBusy,
                finishActions = new List<Action>(1)
                {
                    () => (TargetA.Thing as Building_Computer).TryRemovePart(TargetB.Thing, pawn.Position)
                }
            };
        }   
    }
}
