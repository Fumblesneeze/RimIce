using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Ice
{
    public class CompProperties_HitpointRefuelable : CompProperties_Refuelable
    {
        public CompProperties_HitpointRefuelable()
        {
            compClass = typeof(CompHitpointRefuelable);
        }
    }
}
