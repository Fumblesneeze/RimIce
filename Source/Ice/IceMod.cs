using HugsLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Ice
{
    public class IceMod : ModBase
    {
        private FieldInfo resolvedDesignatorsField;

        public override string ModIdentifier { get; } = "Ice";

        private bool Injected { get; set; }

        public override void Initialize()
        {
            InitReflectionFields();
        }

        public override void DefsLoaded()
        {
        }

        public override void MapLoaded(Map map)
        {
            //map.mapTemperature.MapTemperatureTick();

            //foreach(var trader in DefDatabase<TraderKindDef>.AllDefs)
            //{
            //}

            if(map.terrainGrid.topGrid.Any(x => x.defName == "Ice"))
            {
                InjectDesignators();
            }
            else
            {
                RemoveDesignators();
            }
        }

        private void RemoveDesignators()
        {
            var orders = DefDatabase<DesignationCategoryDef>.GetNamed("Orders", true);

            var resolvedDesignators = (List<Designator>)resolvedDesignatorsField.GetValue(orders);

            var index = resolvedDesignators.FindIndex(item => item is Designator_DigIce);

            if(index >= 0)
            {
                resolvedDesignators.RemoveAt(index);
            }
        }

        private void InitReflectionFields()
        {
            resolvedDesignatorsField = typeof(DesignationCategoryDef).GetField("resolvedDesignators", BindingFlags.NonPublic | BindingFlags.Instance);
            if (resolvedDesignatorsField == null) Logger.Error("failed to reflect DesignationCategoryDef.resolvedDesignators");

        }

        private void InjectDesignators()
        {

            var orders = DefDatabase<DesignationCategoryDef>.GetNamed("Orders", true);

            var resolvedDesignators = (List<Designator>)resolvedDesignatorsField.GetValue(orders);

            var index = resolvedDesignators.FindIndex(item => item is Designator_PlantsHarvestWood);


            var designator = new Designator_DigIce();
            resolvedDesignators.Insert(index + 1, designator);
            //designator.SetVisible(true);
        }
    }
}
