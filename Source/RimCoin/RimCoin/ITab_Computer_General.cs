using RimWorld;
using UnityEngine;
using Verse;

namespace RimCoin
{
    public class ITab_Computer_General : ITab
    {
        public ITab_Computer_General()
        {
            this.size = new Vector2(400f, 300f);
            this.labelKey = "TabComputer";
            this.tutorTag = "Computer";
        }

        Vector2 scrollPosition = Vector2.zero;

        public Building_Computer Computer => this.SelThing as Building_Computer;

        public override bool IsVisible => true;

        protected override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
            Widgets.Label(rect.TopPart(0.10f), "AvailableSpaceTabLabel".Translate(this.Computer.FreeSpace));
            Rect inRect = rect.BottomPart(0.90f);

            Widgets.BeginScrollView(inRect, ref this.scrollPosition, new Rect(0f, 0f, inRect.width, this.Computer.parts.Count * 55f), true);

            float num = 6f;
            Text.Font = GameFont.Medium;
            for (int i = 0; i < this.Computer.parts.Count; i++)
            {
                Widgets.Label(new Rect(0f, num + 5, inRect.width/3f*2f, 40f), this.Computer.parts[i].LabelCap);
                Widgets.Label(new Rect(inRect.width/3f*2f, num + 5, inRect.width/3f, 40f), this.Computer.parts[i].PCPartDef.spaceCost.ToString("{0:##}"));
                Widgets.DrawLineHorizontal(0, num, inRect.width);
                num += 50;
            }
            Widgets.EndScrollView();
        }
    }
}
