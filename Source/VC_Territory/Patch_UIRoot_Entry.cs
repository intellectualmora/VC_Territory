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
    public static class Patch_UIRoot_Entry
    {
        [HarmonyPatch(typeof(UIRoot_Entry), "Init")]
        [HarmonyPostfix]
        public static void Init_Postfix()
        {
            if (!VC_TerritorySetting.isInit)
            {
                VC_TerritorySetting.Init();
            }
            Materials.Init(); // 安全执行图形资源初始化
        }


    }
}

