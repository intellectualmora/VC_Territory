using HarmonyLib;
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

                Color color = new Color(faction.DefaultColor.r, faction.DefaultColor.g, faction.DefaultColor.b,0.7f);
                Material mat = MaterialPool.MatFrom(BaseContent.WhiteTex, ShaderDatabase.WorldOverlayTransparentLit, color, 3600);
                if(!mats.ContainsKey(faction.defName))
                    mats.Add(faction.defName,mat);
        

        }

        private static void GenerateBorderMats(ref Dictionary<string, Material> mats, FactionDef faction)
        {

            Color color = faction.DefaultColor;
            Material mat = MaterialPool.MatFrom(BaseContent.WhiteTex, ShaderDatabase.WorldOverlayTransparentLit, color, 3600);
            if (!mats.ContainsKey(faction.defName))
                mats.Add(faction.defName, mat);

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
