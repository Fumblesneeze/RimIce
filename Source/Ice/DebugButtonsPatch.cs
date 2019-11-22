using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Ice
{
    [HarmonyPatch(typeof(DebugWindowsOpener), "DrawButtons")]
    [StaticConstructorOnStartup]
    static class DebugButtonsPatch
    {
        [HarmonyPatch(typeof(WidgetRow), "ButtonIcon")]
        static class WidgetRowGetter
        {
            private static void Prefix(WidgetRow __instance)
            {
                if (run)
                {
                    run = false;
                    row = __instance;
                }
            }
        }

        private static void Prefix()
        {
            run = true;
        }

        private static void Postfix()
        {
            run = false;
            if (Prefs.DevMode && Current.ProgramState == ProgramState.Playing && row != null)
            {
                Draw();
            }
            row = null;
        }

        private static void Draw()
        {
            if (row.ButtonIcon(butt, "Ice Debug", null))
            {
                DrawDebugOverlay = !DrawDebugOverlay;
            }
        }

        private static Texture2D butt = ContentFinder<Texture2D>.Get("UI/icesaw", true);

        private static WidgetRow row;

        private static bool run;

        public static bool DrawDebugOverlay { get; private set; }
    }
}
