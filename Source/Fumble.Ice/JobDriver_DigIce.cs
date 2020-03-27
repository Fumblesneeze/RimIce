using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Ice
{
    public class JobDriver_DigIce : JobDriver_AffectFloor
    {
        private float workLeft;

        protected override int BaseWorkAmount => 400;

        protected override DesignationDef DesDef => Designations.DoDigIce;

        protected override StatDef SpeedStat => StatDefOf.MiningSpeed;

        protected override void DoEffect(IntVec3 c)
        {
            var thing = ThingMaker.MakeThing(Things.Resource_IceBlocks, null);
            thing.stackCount = 2;

            if(Map.terrainGrid.TerrainAt(c) == IceTerrain.IceShallow)
            {
                var index = Map.cellIndices.CellToIndex(c);
                IceMapComponent.Instance.RemoveIceFromTile(index);
            }

            GenPlace.TryPlaceThing(thing, TargetLocA, Map, ThingPlaceMode.Near, null);
            //Map.terrainGrid.SetTerrain(c, TerrainDef.Named("Ice");
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => (!job.ignoreDesignations && Map.designationManager.DesignationAt(TargetLocA, DesDef) == null) ? true : false);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
            var doWork = new Toil();
            doWork.initAction = delegate
            {
                workLeft = BaseWorkAmount;
            };
            doWork.tickAction = delegate
            {
                float num = (SpeedStat == null) ? 1f : doWork.actor.GetStatValue(SpeedStat);
                workLeft -= num;
                if (doWork.actor.skills != null)
                {
                    doWork.actor.skills.Learn(SkillDefOf.Mining, 0.03f);
                }
                Map.snowGrid.SetDepth(TargetLocA, 0f);
                if (workLeft <= 0f)
                {
                    DoEffect(TargetLocA);
                    Map.designationManager.DesignationAt(TargetLocA, DesDef)?.Delete();
                    ReadyForNextToil();
                }
            };
            doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            doWork.WithProgressBar(TargetIndex.A, () => 1f - workLeft / (float)BaseWorkAmount);
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            doWork.activeSkill = (() => SkillDefOf.Mining);
            yield return doWork;
        }
    }
}