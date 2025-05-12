using System;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using RimWorld.Planet;
using MapModeFramework;
namespace VC_Territory
{
    [HarmonyPatch]
    public static class Patch_Settlement
    {
        [HarmonyPatch(typeof(Settlement), "PostRemove")]//添加据点移除领地移除并刷新地图
        [HarmonyPostfix]
        public static void PostRemove_Postfix(Settlement __instance)
        {
            try
            {
                TerritoryManager.Notify_Influence_Remove(__instance);
                MapModeUI mapModeUI = Find.WindowStack.WindowOfType<MapModeUI>();
                if (mapModeUI != null && mapModeUI.CurrentMapMode is MapMode_Territory mt)
                {
                    MapModeComponent.Instance.RequestMapModeSwitch(mt);
                }
            }
            catch (Exception e)
            {
                Log.Error($"TerritoryPostRemoveFaill: {e}");
                Messages.Message("TerritoryPostRemoveFaill", MessageTypeDefOf.RejectInput, false);

            }
        }

    }
}

