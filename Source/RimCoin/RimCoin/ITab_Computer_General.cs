using System.Linq;
using Reloader;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimCoin
{
    public class ITab_Computer_General : ITab
    {
        public ITab_Computer_General()
        {
            this.size = new Vector2(InspectPaneUtility.PaneWidth, 480f);
            this.labelKey = "RCTabComputer";
            this.tutorTag = "RCComputer";
        }

        static Vector2 partScrollPosition = Vector2.zero;
        static Vector2 slotScrollPosition = Vector2.zero;

        public Building_Computer Computer => this.SelThing as Building_Computer;

        public override bool IsVisible => this.Computer.holdingOwner?.Owner as MinifiedThing == null;

        protected override void FillTab() => 
            FillTabStatic(new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f), this.Computer);

        [ReloadMethod]
        private static void FillTabStatic(Rect rect, Building_Computer computer)
        {
            Widgets.Label(rect.TopPart(0.10f), "RCAvailableSpaceTabLabel".Translate(computer.FreeSpace));

            Rect middleRect = rect.TopPart(0.30f).BottomPart(1f / 3f * 2f);

            if (!(computer.Motherboard?.def is PCMotherboardDef mbd))
                Widgets.Label(middleRect, "RCNoMotherBoardInstalled".Translate());
            else
            {
                Rect slotRect = middleRect.LeftHalf();
                Widgets.Label(slotRect, "RCFreeSlotsTab".Translate());
                Widgets.LabelScrollable(slotRect.BottomPart(0.70f), string.Join("\t", mbd.slots.Select(sce => sce.slot + ": " + computer.FreeSlotCount(sce.slot)).ToArray()), ref slotScrollPosition);

                Rect statRect = middleRect.RightHalf();
                // maybe write some stats here ?
            }

            if (computer.parts.Any())
            {
                Text.Font = GameFont.Tiny;
                Rect partListRect = rect.BottomPart(0.70f);
                Rect partListHeadRect = partListRect.TopPart(0.10f);
                Widgets.Label(partListHeadRect.LeftPart(0.70f), "RCTablePartLabel".Translate());
                Rect rightRect = partListHeadRect.RightPart(0.30f);
                Rect tempRect = rightRect.RightPart(1f / 3f * 2f);
                Widgets.Label(tempRect.LeftHalf(), "RCTableSlotLabel".Translate());
                Widgets.Label(rightRect.LeftPart(1f / 3f), "RCTablePowerLabel".Translate());
                Widgets.Label(tempRect.RightHalf(), "RCTableSpaceLabel".Translate());
                Text.Font = GameFont.Medium;

                Widgets.BeginScrollView(partListRect.BottomPart(0.90f), ref partScrollPosition, new Rect(0f, 0f, partListRect.width, computer.parts.Count * 55f), true);
                float num = 6f;

                for (int i = 0; i < computer.parts.Count; i++)
                {
                    Widgets.DrawLineHorizontal(0, num, partListRect.width);

                    PCPart part = computer.parts[i];

                    Rect partRect = new Rect(0f, num + 5, partListRect.width, 40f);
                    Widgets.Label(partRect.LeftPart(0.70f), part.LabelCap);
                    rightRect = partRect.RightPart(0.30f);
                    tempRect = rightRect.RightPart(1f / 3f * 2f);
                    if (part.def is PCSlotPartDef slotPart)
                        Widgets.Label(tempRect.LeftHalf(), slotPart.slot.ToUpper());
                    Widgets.Label(rightRect.LeftPart(1f / 3f), part.PCPartDef.powerDraw.ToString("#####0"));
                    Widgets.Label(tempRect.RightHalf(), part.PCPartDef.spaceCost.ToString("D2"));
                    TooltipHandler.TipRegion(partRect, () => part.GetInspectString(), part.GetHashCode());
                    num += 50f;
                }
                Widgets.EndScrollView();
            }
        }
    }
}