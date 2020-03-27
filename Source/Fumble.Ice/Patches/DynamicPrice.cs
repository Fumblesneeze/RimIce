using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Ice.Patches
{
    [HarmonyPatch(typeof(Tradeable))]
    [HarmonyPatch(nameof(Tradeable.PriceTypeFor))]
    public class DynamicPrice
    {
        public static void Postfix(Tradeable __instance, ref PriceType __result, TradeAction action)
        {
            if (__instance.ThingDef == Things.Resource_IceBlocks)
            {
                var mapTileIndex = TradeSession.playerNegotiator.Tile;

                var tileTemp = Current.Game.World.tileTemperatures.GetOutdoorTemp(mapTileIndex);

                var isBase = Current.Game.World.worldObjects.AnySettlementAt(mapTileIndex) && Current.Game.World.worldObjects.ObjectsAt(mapTileIndex).Any(x => !x.Faction.IsPlayer);

                var isColony = Current.Game.World.worldObjects.AnySettlementAt(mapTileIndex) && Current.Game.World.worldObjects.ObjectsAt(mapTileIndex).Any(x => x.Faction.IsPlayer);

                if (tileTemp > 50)
                {
                    if (action == TradeAction.PlayerBuys)
                    {
                        __result = PriceType.Exorbitant;
                    }
                    else
                    {
                        __result = PriceType.Normal;
                    }
                }
                else if (tileTemp > 30)
                {
                    if (action == TradeAction.PlayerBuys)
                    {
                        __result = PriceType.Expensive;
                    }
                    else
                    {
                        __result = PriceType.Cheap;
                    }
                }
                else if (tileTemp > 10)
                {
                    if (action == TradeAction.PlayerBuys)
                    {
                        __result = PriceType.Normal;
                    }
                    else
                    {
                        __result = PriceType.VeryCheap;
                    }
                }
                else
                {
                    __result = PriceType.Undefined;
                }
            }
        }
    }
}