using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Ice
{
    public class CompHitpointRefuelable : CompRefuelable
    {
        private CompHeatPusher heatPusher;

        private static PropertyInfo ShouldPushHeatNow { get; } = AccessTools.Property(typeof(CompHeatPusher), "ShouldPushHeatNow");
        private static FieldInfo fuel { get; } = AccessTools.Field(typeof(CompRefuelable), "fuel");

        public CompHitpointRefuelable() : base()
        {
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            heatPusher = parent.GetComp<CompHeatPusher>();
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            if(signal == RefueledSignal)
            {
                var max = parent.MaxHitPoints;

                var shouldHaveHp = (int)Math.Ceiling((float)fuel.GetValue(this) / Props.fuelCapacity * max);
                parent.HitPoints = shouldHaveHp;
            }
        }

        public override string CompInspectStringExtra()
        {
            var curFuel = (float)fuel.GetValue(this);
            var fuelLevel = (int)Math.Round(curFuel / Props.fuelCapacity * 100);

            var text = $"{Props.FuelLabel}: {fuelLevel}%";
            
            if (!this.Props.consumeFuelOnlyWhenUsed && this.HasFuel)
            {
                int numTicks = (int)(curFuel / Props.fuelConsumptionRate * 60000f);
                text = text + " (" + numTicks.ToStringTicksToPeriod() + ")";
            }
            if (!this.HasFuel && !this.Props.outOfFuelMessage.NullOrEmpty())
            {
                text += string.Format("\n{0} ({1}x {2})", this.Props.outOfFuelMessage, this.GetFuelCountToFullyRefuel(), this.Props.fuelFilter.AnyAllowedDef.label);
            }
            if (this.Props.targetFuelLevelConfigurable)
            {
                text = text + "\n" + "ConfiguredTargetFuelLevel".Translate(this.TargetFuelLevel.ToStringDecimalIfSmall());
            }
            return text;
        }

        public override void CompTick()
        {
            var max = parent.MaxHitPoints;
            var hp = parent.HitPoints;
            var shouldHaveFuel = (float) hp / max * Props.fuelCapacity;
            if (shouldHaveFuel < (float)fuel.GetValue(this))
            {
                fuel.SetValue(this, shouldHaveFuel);
            }


            if(heatPusher == null || (bool)ShouldPushHeatNow.GetValue(heatPusher, null))
                base.CompTick();


            var shouldHaveHp = (int)Math.Ceiling((float)fuel.GetValue(this) / Props.fuelCapacity * max);
            if(shouldHaveHp < hp)
            {
                parent.HitPoints = shouldHaveHp;
            }
        }
    }
}
