using HugsLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Ice
{
    public class IceMapComponent : MapComponent
    {
        public IceMapComponent(Map map) : base(map)
        {
            this.EnsureIsActive();
        }

        public float ShallowIceThreshold { get; } = -150;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref IceDepth, "iceDepth", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref TemporarilyRemovedTerrain, "tempTerrain", LookMode.Value, LookMode.Def);
        }

        private Dictionary<int, float> IceDepth = new Dictionary<int, float>();
        private Dictionary<int, TerrainDef> TemporarilyRemovedTerrain = new Dictionary<int, TerrainDef>();

        private int currentIndex;

        private const int TicksPerMap = 360;
        private const int maximumIceDepth = -500;

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            var indexesPerTick = map.cellIndices.NumGridCells / TicksPerMap;

            for (var i = currentIndex; i < map.cellIndices.NumGridCells && i < currentIndex + indexesPerTick; i++)
            {
                var type = map.terrainGrid.topGrid[i];
                var vec = map.cellIndices.IndexToCell(i);
                var temp = map.regionGrid.GetRegionAt_NoRebuild_InvalidAllowed(vec)?.Room?.Temperature ?? map.mapTemperature.OutdoorTemp;

                if (temp < 0)
                {
                    if (CanFreeze(type))
                    {
                        CooldownTile(i, temp, type);
                    }
                }
                else if (type == IceTerrain.IceShallow)
                {
                    WarumupTile(i, temp);
                }
            }
            currentIndex += indexesPerTick;
            if (currentIndex >= map.cellIndices.NumGridCells)
                currentIndex = 0;
        }

        public static bool CanFreeze(TerrainDef type)
        {
            return type == IceTerrain.WaterDeep || type == IceTerrain.WaterShallow || (type.defName.Contains("Water") && !type.defName.Contains("Salt") && !type.defName.Contains("Moving") && !type.defName.Contains("Ocean"));
        }

        private void WarumupTile(int mapIndex, float temp)
        {
            if (!IceDepth.ContainsKey(mapIndex))
            {
                IceDepth.Add(mapIndex, 0f);
            }
            IceDepth[mapIndex] += temp;

            if (IceDepth[mapIndex] > 0)
            {
                var vec = map.cellIndices.IndexToCell(mapIndex);
                if (!TemporarilyRemovedTerrain.ContainsKey(mapIndex))
                {
                    TemporarilyRemovedTerrain.Add(mapIndex, IceTerrain.WaterShallow);
                }
                var oldType = TemporarilyRemovedTerrain[mapIndex];
                TemporarilyRemovedTerrain.Remove(mapIndex);

                map.terrainGrid.SetTerrain(vec, oldType);
                map.designationManager.allDesignations.Remove(
                    map.designationManager.allDesignations.SingleOrDefault(x => x.target == vec && x.def == Designations.DoDigIce)
                );
                IceDepth.Remove(mapIndex);
            }
        }

        private void CooldownTile(int mapIndex, float temp, TerrainDef type)
        {
            var vec = map.cellIndices.IndexToCell(mapIndex);

            var adjacentLandTileCount = GenAdj.CardinalDirections.Select(x => vec + x)
                .Where(x => GenGrid.InBounds(x, map))
                .Select(x => map.terrainGrid.TerrainAt(map.cellIndices.CellToIndex(x)))
                .Where(x => !CanFreeze(x)).Count();

            if (adjacentLandTileCount > 0)
            {
                if (!IceDepth.ContainsKey(mapIndex))
                {
                    IceDepth.Add(mapIndex, 0f);
                }
                IceDepth[mapIndex] += temp / 2 + temp * Rand.Value * adjacentLandTileCount;

                IceDepth[mapIndex] = Math.Max(IceDepth[mapIndex], maximumIceDepth);

                if (IceDepth[mapIndex] < ShallowIceThreshold)
                {
                    if (!TemporarilyRemovedTerrain.ContainsKey(mapIndex))
                    {
                        TemporarilyRemovedTerrain.Add(mapIndex, type);
                    }
                    map.terrainGrid.SetTerrain(vec, IceTerrain.IceShallow);
                }
            }
        }
    }
}