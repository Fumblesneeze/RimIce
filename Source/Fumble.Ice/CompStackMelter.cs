using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Ice
{
    public class CompStackMelter : ThingComp
    {
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

        }

        public override void CompTickRare()
        {
            float ambientTemperature = this.parent.AmbientTemperature;
            if (ambientTemperature < 0f)
            {
                return;
            }
            float f = MeltPerIntervalPer10Degrees * (ambientTemperature / 10f);
            int damage = GenMath.RoundRandom(f);
            if (damage > 0)
            {
                if(parent.HitPoints - damage <= 0.1f && parent.stackCount > 1)
                {
                    parent.stackCount--;
                    parent.HitPoints = parent.MaxHitPoints / 2;
                }
                else
                {
                    parent.TakeDamage(new DamageInfo(DamageDefOf.Rotting, (float)damage, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
                }
            }
        }
        
        private const float MeltPerIntervalPer10Degrees = 0.30f;
    }
}
