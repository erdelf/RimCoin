using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimCoin
{
    public abstract class ShoppingSite : Window
    {
        public WebPageDef def;
    }

    public class ShoppingSite_Grid : ShoppingSite
    {
        public override void DoWindowContents(Rect inRect) => throw new NotImplementedException();
    }

    public class ShoppingSite_List : ShoppingSite
    {
        public override void DoWindowContents(Rect inRect)
        {
            
        }
    }
}