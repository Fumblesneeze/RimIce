using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
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
                    parent.HitPoints = parent.MaxHitPoints;
                }
                else
                {
                    parent.TakeDamage(new DamageInfo(DamageDefOf.Rotting, (float)damage, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
                }
            }
        }

        public override void PreAbsorbStack(Thing otherStack, int count)
        {
            float t = (float)count / (parent.stackCount + count);
            parent.HitPoints = (int)Mathf.Lerp(parent.HitPoints, otherStack.HitPoints, t);
        }

        private const float MeltPerIntervalPer10Degrees = 0.30f;
    }
}
