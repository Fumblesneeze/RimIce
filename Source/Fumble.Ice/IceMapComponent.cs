using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Ice
{
    [StaticConstructorOnStartup]
    public class IceMapComponent : MapComponent
    {
        public static IceMapComponent Instance { get; private set; }

        public IceMapComponent(Map map) : base(map)
        {
            Instance = this;
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref IceDepth, "iceDepth", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref TemporarilyRemovedTerrain, "tempTerrain", LookMode.Value, LookMode.Def);

            if (ThawSpeed == null)
                ThawSpeed = new Dictionary<int, float>();
        }

        private Dictionary<int, float> IceDepth = new Dictionary<int, float>();
        private Dictionary<int, float> ThawSpeed = new Dictionary<int, float>();
        private Dictionary<int, TerrainDef> TemporarilyRemovedTerrain = new Dictionary<int, TerrainDef>();

        private int currentIndex;

        private const int TicksPerMap = 360;
        private const float MinThawRate = 0.9f;
        private const float MaxThawRate = 1f;


        public static float MaximumIceDepthPerIceTile = -300; // should be at least 2x bigger than IceThreshhold
        public static float ShallowIceThreshold = -150;
        private uint ticks;
        private uint nextWarmupTick;

        public override void MapComponentTick()
        {
            if (Instance != this)
            {
                map.components.Remove(this);
                return;
            }
            ticks++;

            var indexesPerTick = map.cellIndices.NumGridCells / TicksPerMap;

            for (var i = currentIndex; i < map.cellIndices.NumGridCells && i < currentIndex + indexesPerTick; i++)
            {
                var type = map.terrainGrid.topGrid[i];
                var vec = map.cellIndices.IndexToCell(i);
                var temp = map.regionGrid.GetRegionAt_NoRebuild_InvalidAllowed(vec)?.Room?.Temperature ?? map.mapTemperature.OutdoorTemp;

                if (temp < 0)
                {
                    if (CanCooldown(type))
                    {
                        CooldownTile(i, temp, type);
                    }
                }
            }
            currentIndex += indexesPerTick;
            if (currentIndex >= map.cellIndices.NumGridCells)
                currentIndex -= map.cellIndices.NumGridCells;

            if (ticks > nextWarmupTick)
            {
                nextWarmupTick += (uint)Rand.RangeInclusive(300, 460);
                foreach (var kvp in IceDepth.ToArray())
                {
                    var i = kvp.Key;
                    var depth = kvp.Value;

                    var vec = map.cellIndices.IndexToCell(i);
                    var temp = map.regionGrid.GetRegionAt_NoRebuild_InvalidAllowed(vec)?.Room?.Temperature ?? map.mapTemperature.OutdoorTemp;

                    if (temp > 0)
                    {
                        WarmupTile(i, temp);
                    }
                }
            }
        }

        public static bool CanCooldown(TerrainDef type)
        {
            return type == IceTerrain.WaterDeep || type == IceTerrain.WaterShallow || type == IceTerrain.Marsh || type == IceTerrain.IceShallow || type == IceTerrain.FrozenMarsh ||
                (type.defName.Contains("Water") && !type.defName.Contains("Salt") && !type.defName.Contains("Moving") && !type.defName.Contains("Ocean"));
        }

        public static bool CanFreeze(TerrainDef type)
        {
            return type == IceTerrain.WaterDeep || type == IceTerrain.WaterShallow || type == IceTerrain.Marsh ||
                (type.defName.Contains("Water") && !type.defName.Contains("Salt") && !type.defName.Contains("Moving") && !type.defName.Contains("Ocean"));
        }

        public static bool IsLand(TerrainDef type)
        {
            return type != IceTerrain.WaterDeep && type != IceTerrain.WaterShallow && type != IceTerrain.Marsh &&
                !type.defName.Contains("Water");
        }

        public static bool IsIce(TerrainDef type)
        {
            return type == IceTerrain.Ice || type == IceTerrain.IceShallow;
        }

        public static bool IsFrozen(TerrainDef type)
        {
            return IsIce(type) || type == IceTerrain.FrozenMarsh;
        }

        private void WarmupTile(int mapIndex, float temp)
        {
            if (!IceDepth.ContainsKey(mapIndex))
            {
                IceDepth.Add(mapIndex, 0f);
            }
            if (!ThawSpeed.ContainsKey(mapIndex))
            {
                ThawSpeed.Add(mapIndex, Rand.Range(MinThawRate, MaxThawRate));
            }
            IceDepth[mapIndex] += temp * ThawSpeed[mapIndex] / 2;

            if (IceDepth[mapIndex] > 0)
            {
                RemoveIceFromTile(mapIndex);
            }
        }

        public override void MapComponentOnGUI()
        {
            if (DebugButtonsPatch.DrawDebugOverlay)
                foreach (var intVec in Find.CameraDriver.CurrentViewRect)
                {
                    if (!GenGrid.InBounds(intVec, map))
                        continue;
                    var i = map.cellIndices.CellToIndex(intVec);
                    var terrain = map.terrainGrid.TerrainAt(i);
                    if (CanCooldown(terrain))
                        if (IceDepth.TryGetValue(i, out var depth))
                        {
                            var v = GenMapUI.LabelDrawPosFor(intVec);
                            GenMapUI.DrawThingLabel(v, Math.Abs(Mathf.RoundToInt(depth)).ToStringCached(), Color.white);
                        }
                }
        }

        public void RemoveIceFromTile(int mapIndex)
        {
            var vec = map.cellIndices.IndexToCell(mapIndex);
            TerrainDef setTerrain = null;

            if (TemporarilyRemovedTerrain.TryGetValue(mapIndex, out var oldType))
            {
                TemporarilyRemovedTerrain.Remove(mapIndex);
                map.terrainGrid.SetTerrain(vec, oldType);
                map.designationManager.allDesignations.Remove(
                    map.designationManager.allDesignations.SingleOrDefault(x => x.target == vec && x.def == Designations.DoDigIce)
                );
                setTerrain = oldType;
            }
            else if (IsFrozen(map.terrainGrid.TerrainAt(mapIndex))) // panic case: should never happen
            {
                map.terrainGrid.SetTerrain(vec, IceTerrain.WaterShallow);
                setTerrain = IceTerrain.WaterShallow;
            }

            ThawSpeed.Remove(mapIndex);
            IceDepth.Remove(mapIndex);

            foreach (var thing in map.thingGrid.ThingsAt(vec).ToArray())
            {
                if (thing is Building && !GenConstruct.TerrainCanSupport(CellRect.SingleCell(vec), map, thing.def))
                {
                    thing.Destroy();
                }
                else if (!(thing is Pawn) && (setTerrain == IceTerrain.WaterDeep || setTerrain.defName.Contains("Deep")))
                {
                    thing.Destroy();
                }
            }
        }

        private void CooldownTile(int mapIndex, float temp, TerrainDef type)
        {
            var vec = map.cellIndices.IndexToCell(mapIndex);

            var adjacentTerrainTiles = GenAdj.AdjacentCells.Select(x => vec + x)
                .Where(x => GenGrid.InBounds(x, map))
                .Select(x => map.terrainGrid.TerrainAt(map.cellIndices.CellToIndex(x)));

            var adjacentLandTileCount = adjacentTerrainTiles
                .Count(x => IsLand(x));


            var isMarsh = type == IceTerrain.Marsh;

            if (isMarsh)
                adjacentLandTileCount = Math.Max(adjacentLandTileCount, 1);

            if (adjacentLandTileCount > 0)
            {
                var adjacentIceTileCount = (float)(adjacentTerrainTiles
                    .Count(x => IsIce(x)));

                var maxIceDepth = Math.Min(MaximumIceDepthPerIceTile * (adjacentIceTileCount / 2f), MaximumIceDepthPerIceTile);

                if (!IceDepth.ContainsKey(mapIndex))
                {
                    IceDepth.Add(mapIndex, 0f);
                }
                if (!ThawSpeed.ContainsKey(mapIndex))
                {
                    ThawSpeed.Add(mapIndex, Rand.Range(MinThawRate, MaxThawRate));
                }

                var newIceDepth = IceDepth[mapIndex] + temp * (Rand.Value + 0.05f) * (adjacentLandTileCount);

                IceDepth[mapIndex] = Math.Max(newIceDepth, maxIceDepth);

                if (!CanFreeze(type))
                    return;

                var factor = type.defName.Contains("Deep") ? 3.0 : 1.0;
                if (isMarsh)
                    factor += 0.5;

                if (IceDepth[mapIndex] < ShallowIceThreshold * factor && !TemporarilyRemovedTerrain.ContainsKey(mapIndex))
                {
                    TemporarilyRemovedTerrain.Add(mapIndex, type);

                    if (isMarsh)
                    {
                        map.terrainGrid.SetTerrain(vec, IceTerrain.FrozenMarsh);
                    }
                    else
                    {
                        map.terrainGrid.SetTerrain(vec, IceTerrain.IceShallow);
                    }
                }
            }
        }
    }
}