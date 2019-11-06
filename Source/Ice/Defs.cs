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

    [DefOf]
    public class Buildings
    {
        public static ThingDef Wall_Ice;
        public static ThingDef IceSculptureSmall;
        public static ThingDef IceSculptureLarge;
        public static ThingDef IceSculptureGrand;
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