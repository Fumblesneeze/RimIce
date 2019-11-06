using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Ice
{
    [HarmonyPatch(typeof(ThingDef))]
    [HarmonyPatch(nameof(ThingDef.HasComp))]
    public static class PatchHasComp
    {
        public static bool Prefix(ThingDef __instance, Type compType, ref bool __result)
        {
            __result = false;
            for (int i = 0; i < __instance.comps.Count; i++)
            {
                if (compType.IsAssignableFrom(__instance.comps[i].compClass))
                {
                    __result = true;
                    break;
                }
            }

            return false;
        }
    }
}
