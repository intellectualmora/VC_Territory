﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VC_Territory
{
   //[StaticConstructorOnStartup]
    public static class Materials
    {
        private const int selectorRenderQueue = 3560;
        private const int terrainOverlayRenderQueue = 3510;

        private const int numMatsGrowingPeriod = 7;

        public static Dictionary<string, Material> matsTerritory;
        public static Dictionary<string, Material> matsBorderTerritory;
        private static Dictionary<string, Material> colorMaterialCache = new Dictionary<string, Material>();

        public static readonly Material matWhiteOverlay;

        public static void Init()
        {
           
            matsTerritory = new Dictionary<string, Material>();
            matsBorderTerritory = new Dictionary<string, Material>();

            foreach (var factiondef in DefDatabase<FactionDef>.AllDefs)
            {
                GenerateMats(ref matsTerritory, factiondef);
                GenerateBorderMats(ref matsBorderTerritory, factiondef);
            }
            
        }
        static Materials()
        {
           
        }

        private static void GenerateMats(ref Dictionary<string, Material> mats,FactionDef faction)
        {
            Color color;
            if (VC_TerritorySetting.factionColorDict != null)
            {
                if (VC_TerritorySetting.factionColorDict.ContainsKey(faction))
                {
                    color = new Color(VC_TerritorySetting.factionColorDict[faction].r, VC_TerritorySetting.factionColorDict[faction].g, VC_TerritorySetting.factionColorDict[faction].b, 0.7f);
                }
                else
                {
                    color = new Color(faction.DefaultColor.r, faction.DefaultColor.g, faction.DefaultColor.b, 0.7f);
                }
            }
            else
            {
                color = new Color(faction.DefaultColor.r, faction.DefaultColor.g, faction.DefaultColor.b, 0.7f);

            }
            string colorKey = $"{color.r:F2},{color.g:F2},{color.b:F2},{color.a:F2}";
            if (!colorMaterialCache.TryGetValue(colorKey, out Material mat))
            {
                mat = MaterialPool.MatFrom(BaseContent.WhiteTex, ShaderDatabase.WorldOverlayTransparentLit, color, 3600);
                colorMaterialCache[colorKey] = mat;
            }
            mats[faction.defName] = mat;
        }

        private static void GenerateBorderMats(ref Dictionary<string, Material> mats, FactionDef faction)
        {
            Color color;
            if (VC_TerritorySetting.factionColorDict != null)
            {
                if (VC_TerritorySetting.factionColorDict.ContainsKey(faction))
                {
                    color = VC_TerritorySetting.factionColorDict[faction];
                }
                else
                {
                    color = faction.DefaultColor;
                }
            }
            else
            {
                color = faction.DefaultColor;

            }
            string colorKey = $"{color.r:F2},{color.g:F2},{color.b:F2},{color.a:F2}";
            if (!colorMaterialCache.TryGetValue(colorKey, out Material mat))
            {
                mat = MaterialPool.MatFrom(BaseContent.WhiteTex, ShaderDatabase.WorldOverlayTransparentLit, color, 3600);
                colorMaterialCache[colorKey] = mat;
            }
            mats[faction.defName] = mat;

        }

        public static Material MatForTerritory(Territory territory)
        {
            if (territory == null)
            {
                return BaseContent.ClearMat;
            }
            return matsTerritory[territory.factionDef.defName];

        }


     
    }
}
