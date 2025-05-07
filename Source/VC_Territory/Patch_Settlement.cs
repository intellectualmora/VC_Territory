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
        [HarmonyPatch(typeof(Settlement), "PostRemove")]
        [HarmonyPostfix]
        public static void PostRemove_Postfix(Settlement __instance)
        {
            TerritoryManager.Notify_Influence_Remove(__instance);            
            MapModeUI mapModeUI = Find.WindowStack.WindowOfType<MapModeUI>();
            if(mapModeUI.CurrentMapMode is MapMode_Territory)
            {
                MapMode_Territory mt = (MapMode_Territory)mapModeUI.CurrentMapMode;
                MapModeComponent.Instance.RequestMapModeSwitch(mt);
            }
        }

    }
}

