using Harmony;
using System.Reflection;

namespace Ice.Patches
{
    public static class CheckAndPatch
    {
        public static void PatchMethods()
        {
            var harmony = HarmonyInstance.Create("Fumblesneeze.Ice");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}