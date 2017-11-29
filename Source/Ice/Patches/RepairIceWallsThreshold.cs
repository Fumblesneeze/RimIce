using Harmony;
using RimWorld;
using Verse;

namespace Ice.Patches
{
    [HarmonyPatch(typeof(WorkGiver_Repair))]
    [HarmonyPatch(nameof(WorkGiver_Repair.HasJobOnThing))]
    public static class RepairIceWallsThreshold
    {
        public static double RepairThreshold = 0.9;

        public static void Postfix(Pawn pawn, Thing t, bool forced, WorkGiver_Repair __instance, ref bool __result)
        {
            if (__result == false)
                return;

            if (t.def == Buildings.Cooler_Wall_Ice ||
                t.def == Buildings.Wall_Ice ||
                t.def == Buildings.IceSculptureSmall ||
                t.def == Buildings.IceSculptureLarge)
            {
                if ((double)t.HitPoints / t.MaxHitPoints > RepairThreshold)
                {
                    __result = forced;
                }
            }
        }
    }
}