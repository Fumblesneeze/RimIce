using RimWorld;
using Verse;

namespace Ice
{
    [DefOf]
    public class IceTerrain
    {
        public static TerrainDef WaterShallow;
        public static TerrainDef WaterDeep;
        public static TerrainDef Ice;
        public static TerrainDef IceShallow;
    }

    public class Traders
    {
        public static TraderKindDef BulkGoods;
    }

    [DefOf]
    public class Designations
    {
        public static DesignationDef DoDigIce;
    }

    [DefOf]
    public class Jobs
    {
        public static JobDef DigIce;
    }

    [DefOf]
    public class DesignationCategories
    {
        public static DesignationCategoryDef Orders;
    }

    [DefOf]
    public class Things
    {
        public static ThingDef Resource_IceBlocks;
    }
}