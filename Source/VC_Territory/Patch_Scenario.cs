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
    public static class Patch_Scenario
    {
        [HarmonyPatch(typeof(Scenario), "PreMapGenerate")]
        [HarmonyPrefix]
        public static void PreMapGenerate_Prefix()
        {
            if (Current.Game != null && Current.Game?.InitData != null)
            {
                if (Current.Game.InitData.QuickStarted)
                {
                    TerritoryManager.InfluenceInitize();
                    Log.Message("开发者模式初始完毕:" + TerritoryManager.territories.Count);
                    return;
                }
                return;
            }
            Log.Message("加载完毕:" + TerritoryManager.territories.Count);
          
        }

    }
}

