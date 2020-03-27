using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Ice
{
    public class IceStockGenerator : StockGenerator
    {
        public IceStockGenerator()
        {
            countRange = new IntRange(25, 400);
            totalPriceRange = new FloatRange(0f, 700f);
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef == Things.Resource_IceBlocks;
        }

        public override IEnumerable<Thing> GenerateThings(int mapTileIndex, Faction faction)
        {
            var tileTemp = Current.Game.World.tileTemperatures.GetOutdoorTemp(mapTileIndex);

            var isColony = Current.Game.World.worldObjects.AnySettlementAt(mapTileIndex);

            foreach (var thing in StockGeneratorUtility.TryMakeForStock(Things.Resource_IceBlocks, RandomCountOf(Things.Resource_IceBlocks)))
            {
                if (!isColony)
                    yield return thing;
            }
        }
    }
}