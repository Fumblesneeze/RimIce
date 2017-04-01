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
}
