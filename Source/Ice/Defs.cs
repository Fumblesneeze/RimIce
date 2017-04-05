using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Ice
{
    [DefOf]
    public class IceTerrainDefs
    {
        public static TerrainDef WaterShallow;
        public static TerrainDef WaterDeep;
        public static TerrainDef SaltWaterShallow;
        public static TerrainDef SaltWaterDeep;
        public static TerrainDef Ice;
        public static TerrainDef IceShallow;
    }

    [DefOf]
    public class Traders
    {
        public static TraderKindDef BulkGoods;
    }

    public class DynamicTerrainDefs
    {
        public static TerrainDef SaltWaterModerate;
    }

    [DefOf]
    public class Things
    {
        public static ThingDef Resource_IceBlocks;
    }
}
