using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimCoin
{
    static class WebPageUtility
    {
        public static void OpenDialog() => Find.WindowStack.Add(new Dialog_InternetStartPage());
    }

    public class Dialog_InternetStartPage : Window
    {
        public Dialog_InternetStartPage() : base()
        {
            this.optionalTitle = "InternetWindowTitle".Translate();
            this.doCloseX = true;
            this.doCloseButton = true;
            this.forcePause = true; // don't know yet
        }

        protected override void SetInitialSizeAndPosition() => base.SetInitialSizeAndPosition();

        public override void DoWindowContents(Rect inRect)
        {
            throw new NotImplementedException();
        }
    }

    public class WebPageDef : Def
    {
        public Type webPageClass;
        public List<StockGenerator> stockGenerators;
        public IEnumerable<Thing> things;
        public ThingDef[] thingDefs;

        public override void PostLoad()
        {
            base.PostLoad();
            this.things = this.stockGenerators.SelectMany(sg => sg.GenerateThings(0));

            this.thingDefs = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(ThingDef)) && !t.IsAbstract).Concat(new Type[1] { typeof(ThingDef) })
                    .SelectMany(t => ((IEnumerable)typeof(DefDatabase<>).MakeGenericType(t).GetProperty(nameof(DefDatabase<Def>.AllDefs)).GetGetMethod().Invoke(null, null)).Cast<ThingDef>())
                    .Where(td => this.stockGenerators.Any(sg => sg.HandlesThingDef(td))).ToArray();
            Log.Message( AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(ThingDef)) && !t.IsAbstract).Count() + "\n" + string.Join( " | ", AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(ThingDef)) && !t.IsAbstract).Select(td => td.Name).ToArray()) +
                "\n" + AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(ThingDef)) && !t.IsAbstract).Concat(new Type[1] { typeof(ThingDef) })
                    .SelectMany(t => ((IEnumerable)typeof(DefDatabase<>).MakeGenericType(t).GetProperty(nameof(DefDatabase<Def>.AllDefs)).GetGetMethod().Invoke(null, null)).Cast<ThingDef>()).Count() +
                "\n" + this.thingDefs.Count().ToString());

            Log.Message(AppDomain.CurrentDomain.GetAssemblies().Count() + " Assemblies in current AppDomain\n" + string.Join(" | ", AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name).ToArray()));
            Log.Message(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Count() + " Types in current AppDomain\n" + 
                string.Join(" | ", AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Select(t => t.Name).ToArray()));
            Log.Message(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).Count() + " abstract Types in current Domain\n" + 
                string.Join(" | ", AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).Select(t => t.Name).ToArray()));
            Log.Message(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ThingDef))).Count() + " abstract subclass of ThingDef Types in current Domain\n" + 
                string.Join(" | ", AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ThingDef))).Select(t => t.Name).ToArray()));
            Log.Message(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && typeof(ThingDef).IsAssignableFrom(t)).Count() + " abstract assignables of ThingDef Types in current Domain\n" + 
                string.Join(" | ", AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && typeof(ThingDef).IsAssignableFrom(t)).Select(t => t.Name).ToArray()));
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string s in base.ConfigErrors())
                yield return s;
            if(this.webPageClass == null)
                yield return this.defName + " has null thingClass.";
            if (!typeof(Window).IsAssignableFrom(this.webPageClass))
                yield return this.defName + " has non-Window type as thingClass: " + this.webPageClass.FullName;
        }
    }
}