using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.Noise;

namespace Ice
{
	public class BeachMakerPatch
	{
		public static Traverse beachNoise = Traverse.CreateWithType("BeachMaker").Field("beachNoise");

		public static TerrainDef BeachTerrainAt(IntVec3 loc)
		{
			var val = beachNoise.GetValue<ModuleBase>();
			if (val == null)
				return (TerrainDef)null;
			float num = val.GetValue(loc);
			if ((double)num < 0.100000001490116)
				return IceTerrainDefs.SaltWaterDeep;
			if ((double)num < 0.449999988079071)
				return IceTerrainDefs.SaltWaterShallow;
			if ((double)num < 1.0)
				return TerrainDefOf.Sand;
			return (TerrainDef)null;
		}
	}
	public class BT_BeachMakerPatch
	{
		public static Traverse beachNoise = Traverse.CreateWithType("BT_BeachMaker").Field("beachNoise");

		public static TerrainDef BeachTerrainAt(IntVec3 loc)
		{
			var val = beachNoise.GetValue<ModuleBase>();
			if (val == null)
				return (TerrainDef)null;
			float num = val.GetValue(loc);
			if ((double)num < 0.100000001490116)
				return IceTerrainDefs.SaltWaterDeep;
			if ((double)num < 0.449999988079071)
				return IceTerrainDefs.SaltWaterShallow;
			if ((double)num < 1.0)
				return TerrainDefOf.Sand;
			return (TerrainDef)null;
		}
	}

	public class RFF_BeachMakerPatch
	{
		public static Traverse beachNoise = Traverse.CreateWithType("RFF_BeachMaker").Field("beachNoise");

		public static TerrainDef BeachTerrainAt(IntVec3 loc)
		{
			TerrainDef result;
			if (beachNoise.GetValue() == null)
			{
				result = null;
			}
			else
			{
				float value = beachNoise.GetValue<ModuleBase>().GetValue(loc);
				if (value < 0.05f)
				{
					result = IceTerrainDefs.SaltWaterDeep;
				}
				else if (value < 0.25f)
				{
					result = DynamicTerrainDefs.SaltWaterModerate;
				}
				else if (value < 0.45f)
				{
					result = IceTerrainDefs.SaltWaterShallow;
				}
				else if (value < 1f)
				{
					result = TerrainDefOf.Sand;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
	}
}
