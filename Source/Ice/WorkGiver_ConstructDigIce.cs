// Decompiled with JetBrains decompiler
// Type: mars.WorkGiver_ConstructDigSand
// Assembly: mars, Version=0.10.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E731A3F2-752D-44AC-AF37-ABC49F25D7B2
// Assembly location: F:\SteamLibrary\steamapps\workshop\content\294100\732754899\Assemblies\mars.dll

using RimWorld;
using Verse;
using Verse.AI;

namespace Ice
{
    public class WorkGiver_ConstructDigIce : WorkGiver_ConstructAffectFloor
    {
        protected override DesignationDef DesDef
        {
            get
            {
                return DefDatabase<DesignationDef>.GetNamed("DoDigIce", true);
            }
        }

        public WorkGiver_ConstructDigIce() : base()
        {
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 c)
        {
            return new Job(DefDatabase<JobDef>.GetNamed("DigIce", true), c);
        }
    }
}
