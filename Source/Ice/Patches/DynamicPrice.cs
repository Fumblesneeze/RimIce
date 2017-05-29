using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                var isBase = Current.Game.World.worldObjects.AnyFactionBaseAt(mapTileIndex);

                var isColony = Current.Game.World.worldObjects.AnySettlementAt(mapTileIndex);

                if (action == TradeAction.PlayerBuys)
                {
                    if (isColony)
                    {
                        // ice trader
                        __result = PriceType.Normal;
                    }
                    else if (isBase)
                    {
                        // buy from faction base
                        __result = (PriceType)(byte)Math.Round(Math.Exp(tileTemp / 20)); // for celsius degrees
                    }
                }
                else
                {
                    if (isColony)
                    {
                        // caravan buys at this cost
                        __result = (PriceType)(byte)(tileTemp < 0 ? 1 : tileTemp < 20 ? 2 : tileTemp < 30 ? 3 : 4);
                    }
                    else if (isBase)
                    {
                        // sell to faction base
                        __result = (PriceType)(byte)Math.Round(Math.Exp(tileTemp / 20) / 2); // for celsius degrees
                    }
                }


            }

        }


    }
}
