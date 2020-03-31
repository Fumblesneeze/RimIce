using HarmonyLib;
using Ice;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Fumble.Ice.Patches
{
    [HarmonyPatch(typeof(CompRottable))]
    [HarmonyPatch(nameof(CompRottable.Active), MethodType.Getter)]
    public static class RottableCaravan
    {
        public static bool Prefix(CompRottable __instance, ref bool __result)
        {
            __result = true;
            var parentHolder = __instance.parent.ParentHolder;
            while (parentHolder != null && !(parentHolder is Map))
            {
                if (parentHolder.GetDirectlyHeldThings()?.Any(thing => thing.def == Things.Resource_IceBlocks) ?? false)
                {
                    __result = false;
                    return false;
                }
                if (parentHolder is ThingOwner<Pawn> thingOwner && thingOwner.Owner is Caravan c)
                {
                    if (c.AllThings.Any(thing => thing.def == Things.Resource_IceBlocks))
                    {
                        __result = false;
                        return false;
                    }
                }
                parentHolder = parentHolder.ParentHolder;
            }

            return true;
        }
    }
}
