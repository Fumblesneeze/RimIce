using HarmonyLib;
using System.Reflection;

namespace Ice.Patches
{
    public static class CheckAndPatch
    {
        public static void PatchMethods()
        {
            var harmony = new Harmony("Fumble.Ice");
            harmony.PatchAll();
        }
    }
}