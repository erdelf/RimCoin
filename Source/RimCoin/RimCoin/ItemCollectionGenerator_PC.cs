using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reloader;
using RimWorld;
using Verse;

namespace RimCoin
{
    class ItemCollectionGenerator_RimCoin : ItemCollectionGenerator
    {
        protected override void Generate(ItemCollectionGeneratorParams parms, List<Thing> outThings) =>
            outThings.AddRange( new HashSet<ThingDef>(DefDatabase<PCCaseDef>.AllDefs.Cast<ThingDef>().Concat(
                typeof(ItemCollectionGenerator_RimCoin).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ThingDef))).SelectMany(t => t.AllSubclassesNonAbstract())
                    .SelectMany(t => ((IEnumerable)typeof(DefDatabase<>).MakeGenericType(t).GetProperty("AllDefs").GetGetMethod().Invoke(null, null)).Cast<ThingDef>()))).
                        Select(d => ThingMaker.MakeThing(d, GenStuff.RandomStuffFor(d))));
    }
}