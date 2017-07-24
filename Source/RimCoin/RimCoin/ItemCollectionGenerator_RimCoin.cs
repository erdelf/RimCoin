using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace RimCoin
{
    class ItemCollectionGenerator_RimCoin : ItemCollectionGenerator
    {
        protected override void Generate(ItemCollectionGeneratorParams parms, List<Thing> outThings) =>
            outThings.AddRange(new HashSet<ThingDef>(Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ThingDef))).SelectMany(t => t.AllSubclassesNonAbstract().Concat(new Type[1] { t }))
                    .SelectMany(t => ((IEnumerable)typeof(DefDatabase<>).MakeGenericType(t).GetProperty(nameof(DefDatabase<Def>.AllDefs)).GetGetMethod().Invoke(null, null)).Cast<ThingDef>())).
                        Select(d => ThingMaker.MakeThing(d, GenStuff.RandomStuffFor(d))));
    }
}