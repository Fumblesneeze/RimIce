﻿using RimWorld;
using Verse;
using Verse.AI;

namespace Ice
{
    public class WorkGiver_ConstructDigIce : WorkGiver_ConstructAffectFloor
    {
        protected override DesignationDef DesDef => Designations.DoDigIce;

        public override Job JobOnCell(Pawn pawn, IntVec3 cell, bool forced = false) => new Job(Jobs.DigIce, cell);
    }
}