using Harmony;
using HugsLib;
using HugsLib.Source.Detour;
using HugsLib.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Ice
{
	[DefOf]
	public class IceTerrainDefs
	{
		public static TerrainDef WaterShallow;
		public static TerrainDef WaterDeep;
		public static TerrainDef SaltWaterShallow;
		public static TerrainDef SaltWaterDeep;
		public static TerrainDef Ice;
		public static TerrainDef IceShallow;
	}

	public class DynamicTerrainDefs
	{
		public static TerrainDef SaltWaterModerate;
	}

	public class IceMod : ModBase
	{
		private FieldInfo resolvedDesignatorsField;
		private IceMapComponent iceComponent;

		public override string ModIdentifier { get; } = "Ice";

		public override void Initialize()
		{
			InitReflectionFields();

			DetourProvider.CompatibleDetour(AccessTools.TypeByName("BeachMaker").GetMethod("BeachTerrainAt"),
				typeof(BeachMakerPatch).GetMethod(nameof(BeachMakerPatch.BeachTerrainAt)));


			if(LoadedModManager.RunningMods.Any(x => x.Name == "Better Terrain"))
			{
				Log.Message("Ice: inject compatibility for BetterTerrain");
				var betterTerrain = AccessTools.TypeByName("BT_BeachMaker");
				if (betterTerrain != null)
				{
					DetourProvider.CompatibleDetour(betterTerrain.GetMethod("BeachTerrainAt"),
						typeof(BT_BeachMakerPatch).GetMethod(nameof(BT_BeachMakerPatch.BeachTerrainAt)));
				}
			}

			if (LoadedModManager.RunningMods.Any(x => x.AllDefs.Any(y => y.defName == "WaterModerate")))
			{
				var def = DefDatabase<TerrainDef>.GetNamed("WaterModerate");

				var saltWaterDef = new TerrainDef { defName = "SaltWaterModerate" };

				AccessTools.GetFieldNames(def).Select(x => AccessTools.Field(def.GetType(), x)).Do(x => x.SetValue(saltWaterDef, x.GetValue(def)));

				saltWaterDef.defName = "SaltWaterModerate";
				saltWaterDef.label = "SaltWaterModerate.label".Translate();

				DefDatabase<TerrainDef>.Add(saltWaterDef);
				DynamicTerrainDefs.SaltWaterModerate = saltWaterDef;

				Log.Message("Ice: inject compatibility for FertileFields");
				var fertileFields = AccessTools.TypeByName("RFF_BeachMaker");
				if (fertileFields != null)
				{
					DetourProvider.CompatibleDetour(fertileFields.GetMethod("BeachTerrainAt"),
						typeof(RFF_BeachMakerPatch).GetMethod(nameof(RFF_BeachMakerPatch.BeachTerrainAt)));
				}
			}
		}

		public override void DefsLoaded()
		{
		}

		public override void MapLoaded(Map map)
		{
			InjectDesignators();

			iceComponent = MapComponentUtility.GetMapComponent<IceMapComponent>(map);
			if (iceComponent == null)
				iceComponent = new IceMapComponent(map);
		}

		private void RemoveDesignators()
		{
			var orders = DefDatabase<DesignationCategoryDef>.GetNamed("Orders", true);

			var resolvedDesignators = (List<Designator>)resolvedDesignatorsField.GetValue(orders);

			var index = resolvedDesignators.FindIndex(item => item is Designator_DigIce);

			if (index >= 0)
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

			if (resolvedDesignators.FindIndex(item => item is Designator_DigIce) >= 0)
				return;

			var index = resolvedDesignators.FindIndex(item => item is Designator_PlantsHarvestWood);


			var designator = new Designator_DigIce();
			resolvedDesignators.Insert(index + 1, designator);
			//designator.SetVisible(true);
		}
	}
}
