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
            this.labelKey = "TabComputer";
            this.tutorTag = "Computer";
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
            Widgets.Label(rect.TopPart(0.10f), "AvailableSpaceTabLabel".Translate(computer.FreeSpace));

            Rect middleRect = rect.TopPart(0.30f).BottomPart(0.66f);

            if (!(computer.Motherboard?.def is PCMotherboardDef mbd))
                Widgets.Label(middleRect, "NoMotherBoardInstalled".Translate());
            else
            {
                Rect slotRect = middleRect.LeftHalf();
                Widgets.Label(slotRect, "FreeSlotsTab".Translate());
                Widgets.LabelScrollable(slotRect.BottomPart(0.70f), string.Join("\t", mbd.slots.Select(sce => sce.slot + ": " + computer.FreeSlotCount(sce.slot)).ToArray()), ref slotScrollPosition);

                Rect statRect = middleRect.RightHalf();
                // maybe do something here ?
            }

            Rect partListRect = rect.BottomPart(0.70f);

            Widgets.BeginScrollView(partListRect, ref partScrollPosition, new Rect(0f, 0f, partListRect.width, computer.parts.Count * 55f), true);

            float num = 6f;
            Text.Font = GameFont.Medium;
            for (int i = 0; i < computer.parts.Count; i++)
            {
                Widgets.DrawLineHorizontal(0, num, partListRect.width);

                PCPart part = computer.parts[i];

                Rect partRect = new Rect(0f, num + 5, partListRect.width, 40f);
                Widgets.Label(partRect.LeftPart(0.80f), part.LabelCap);
                Widgets.Label(partRect.RightPart(0.20f), part.PCPartDef.spaceCost.ToString("D2"));
                TooltipHandler.TipRegion(partRect, () => part.GetInspectString(), part.GetHashCode());
                num += 50f;
            }
            Widgets.EndScrollView();
        }
    }
}