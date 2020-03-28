using HarmonyLib;
using HugsLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Ice
{
    public class IceMod : ModBase
    {
        public override string ModIdentifier { get; } = "Fumble.Ice";

        public override void Initialize()
        {
        }
        private bool added = false;

        public override void MapLoaded(Map map)
        {
            if (!added)
            {
                AddDesignators();
                added = true;
            }
        }

        private void AddDesignators()
        {
            var orders = Traverse.Create(DesignationCategories.Orders).Field("resolvedDesignators").GetValue<List<Designator>>();

            if (orders.Any(item => item is Designator_DigIce))
                return;

            var index = orders.FindIndex(item => item is Designator_PlantsHarvestWood);

            var designator = new Designator_DigIce();
            orders.Insert(index + 1, designator);
        }
    }
}