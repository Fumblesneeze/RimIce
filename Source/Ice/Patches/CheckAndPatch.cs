using Harmony;
using RimWorld;
using System.Reflection;

namespace Ice.Patches
{
    public static class CheckAndPatch
    {
        public static void InjectDetours()
        {
            var harmony = HarmonyInstance.Create("Fumblesneeze.Ice");

            //harmony.Patch(typeof(TraderStockGenerator).GetMethod(nameof(TraderStockGenerator.GenerateTraderThings)),
            //    null,
            //    null
            //    );

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}