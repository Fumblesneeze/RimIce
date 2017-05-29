using RimWorld;
using Verse;

namespace Ice
{
    public class JobDriver_DigIce : JobDriver_AffectFloor
    {
        protected override int BaseWorkAmount => 400;

        protected override DesignationDef DesDef => Designations.DoDigIce;

        protected override StatDef SpeedStat => StatDefOf.MiningSpeed;

        protected override void DoEffect(IntVec3 c)
        {
            var thing = ThingMaker.MakeThing(Things.Resource_IceBlocks, null);
            thing.stackCount = 2;

            GenPlace.TryPlaceThing(thing, TargetLocA, Map, ThingPlaceMode.Near, null);

            //Map.terrainGrid.SetTerrain(c, TerrainDef.Named("Ice");
        }
    }
}