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
    [HarmonyPatch(typeof(Page_SelectStartingSite), nameof(Page_SelectStartingSite.PostOpen))]//新建游戏初始化位点
    public static class Page_SelectStartingSite_PostOpen
    {

        public static void Postfix()
        {
            if (Current.Game != null && Current.Game?.InitData != null) 
            {
                TerritoryManager.InfluenceInit();
                Log.Message("初始化完毕:" + TerritoryManager.territories.Count);
            }
        }
    }
}

