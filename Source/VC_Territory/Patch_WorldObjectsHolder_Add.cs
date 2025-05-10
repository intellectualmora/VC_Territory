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
    public static class Patch_WorldObjectsHolder_Add
    {
        [HarmonyPatch(typeof(WorldObjectsHolder), "Add")] //添加据点触发领地增加并刷新地图
        [HarmonyPostfix]
        public static void Postfix(WorldObject o)
        {
            if (o is Settlement settlement)
            {
                Faction playerSilentFail = Faction.OfPlayerSilentFail;
                if (playerSilentFail != null)
                {
                    TerritoryManager.Notify_Influence_Add(settlement);
                    try
                    {
                        MapModeUI mapModeUI = Find.WindowStack.WindowOfType<MapModeUI>();
                        if (mapModeUI.CurrentMapMode is MapMode_Territory)
                        {
                            MapMode_Territory mt = (MapMode_Territory)mapModeUI.CurrentMapMode;
                            MapModeComponent.Instance.RequestMapModeSwitch(mt);

                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }
       

    }

}

