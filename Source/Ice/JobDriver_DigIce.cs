using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Ice
{
    public class JobDriver_DigIce : JobDriver_AffectFloor
    {
        protected override int BaseWorkAmount
        {
            get
            {
                return 700;
            }
        }

        protected override DesignationDef DesDef
        {
            get
            {
                return DefDatabase<DesignationDef>.GetNamed("DoDigIce", true);
            }
        }

        protected override StatDef SpeedStat
        {
            get
            {
                return StatDefOf.ConstructionSpeed;
            }
        }

        public JobDriver_DigIce() : base()
        {
        }

        protected override void DoEffect(IntVec3 c)
        {
                var thing = ThingMaker.MakeThing(ThingDef.Named("Resource_IceBlocks"), null);
                thing.stackCount = 1;

                GenPlace.TryPlaceThing(thing, TargetLocA, Map, ThingPlaceMode.Near, null);
                //Map.terrainGrid.SetTerrain(c, TerrainDef.Named("Ice");
        }
    }
}
