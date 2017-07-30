using System;
using Verse;

namespace RimCoin
{
    static class WebPageUtility
    {
        public static void OpenDialog() => Find.WindowStack.Add(new Dialog_DebugLogMenu());
    }
}