using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Ice
{
    public class IceStockGenerator : StockGenerator
    {
        public IceStockGenerator()
        {
            countRange = new IntRange(25,400);
            totalPriceRange = new FloatRange(0f, 700f);
        }

        public override IEnumerable<Thing> GenerateThings(Map forMap)
        {
            var things = new Thing { def = Things.Resource_IceBlocks, stackCount = RandomCountOf(Things.Resource_IceBlocks) };

            yield return things;
        }

        public new bool TryGetPriceType(ThingDef thingDef, TradeAction action, out PriceType priceType)
        {
            Log.Message($"Trying to get price of {thingDef}");
            if (!this.HandlesThingDef(thingDef))
            {
                priceType = PriceType.Undefined;
                return false;
            }
            priceType = this.price;
            return true;
        }


        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef == Things.Resource_IceBlocks;
        }
    }
}
