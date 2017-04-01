using HugsLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Ice
{
	public class IceMapComponent : MapComponent
	{
		public IceMapComponent(Map map) : base(map)
		{
			this.EnsureIsActive();
		}

		public double ShallowIceThreshold { get; private set; } = -150;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.LookDictionary(ref IceDepth, "iceDepth", LookMode.Value, LookMode.Value);
			Scribe_Collections.LookDictionary(ref TemporarilyRemovedTerrain, "tempTerrain", LookMode.Value, LookMode.Def);
		}

		private Dictionary<int, double> IceDepth = new Dictionary<int, double>();

		private Dictionary<int, TerrainDef> TemporarilyRemovedTerrain = new Dictionary<int, TerrainDef>();
		private int currentIndex;
		private const int TicksPerMap = 120;

		public override void MapComponentTick()
		{
			base.MapComponentTick();
			var temp = map.mapTemperature.OutdoorTemp;

			var indexesPerTick = map.cellIndices.NumGridCells / TicksPerMap;

			for (var i = currentIndex; i < map.cellIndices.NumGridCells && i < currentIndex + indexesPerTick; i++)
			{
				var type = map.terrainGrid.topGrid[i];

				if (type == IceTerrainDefs.WaterDeep || type == IceTerrainDefs.WaterShallow || (type.defName.Contains("Water") && !type.defName.Contains("Salt")))
				{
					if(temp < 0)
					{
						var vec = map.cellIndices.IndexToCell(i);

						var hasAdjacentLand = GenAdj.CardinalDirections.Select(x => vec + x)
							.Where(x => GenGrid.InBounds(x, map))
							.Select(x => map.terrainGrid.TerrainAt(map.cellIndices.CellToIndex(x)))
							.Where(x => !x.defName.Contains("Water")).Any();

						if (hasAdjacentLand)
						{
							if (!IceDepth.ContainsKey(i))
							{
								IceDepth.Add(i, 0f);
							}
							IceDepth[i] += temp/2 + temp * Rand.Value;

							if (IceDepth[i] < ShallowIceThreshold)
							{
								TemporarilyRemovedTerrain.Add(i, type);
								map.terrainGrid.SetTerrain(vec, IceTerrainDefs.IceShallow);
							}
						}
					}
				}
				else if (type == IceTerrainDefs.IceShallow)
				{
					if (!IceDepth.ContainsKey(i))
					{
						IceDepth.Add(i, 0f);
					}
					IceDepth[i] += temp;

					if (IceDepth[i] > 0)
					{
						var vec = map.cellIndices.IndexToCell(i);
						if (!TemporarilyRemovedTerrain.ContainsKey(i))
						{
							TemporarilyRemovedTerrain.Add(i, IceTerrainDefs.WaterShallow);
						}
						var oldType = TemporarilyRemovedTerrain[i];
						TemporarilyRemovedTerrain.Remove(i);

						map.terrainGrid.SetTerrain(vec, oldType);
						IceDepth.Remove(i);
					}
				}
			}
			currentIndex += indexesPerTick;
			if (currentIndex >= map.cellIndices.NumGridCells)
				currentIndex = 0;
		}
	}
}
