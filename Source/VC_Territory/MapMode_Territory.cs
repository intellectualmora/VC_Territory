using MapModeFramework;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Verse;

namespace VC_Territory
{
    public class MapMode_Territory : MapMode_Region
    {
        public override WorldLayer_MapMode WorldLayer => WorldLayer_MapMode_Territory.Instance;
        public override bool CanToggleWater => true;
        public override Material RegionMaterial => BaseContent.ClearMat;
        public Dictionary<string, Material> FactionMaterials => Materials.matsTerritory;
        public Dictionary<string, Material> FactionBorderMaterials => Materials.matsBorderTerritory;
        public override bool DoBorders => true;
        public MapMode_Territory() { }
        public MapMode_Territory(MapModeDef def) : base(def) { }

        public override void Initialize()
        {
            base.Initialize();
            
        }
        public override Material GetMaterial(int tile)
        {
            return VC_Territory.Materials.MatForTerritory(TerritoryManager.territories.Find(t => t.tileId == tile));
        } 
        public override string GetTileLabel(int tile)
        {
            Territory t = TerritoryManager.GetTerritory(tile);
            if (t != null)
                return t.settlement.Name;
            else return "";
        }
        public override string GetTooltip(int tile)
        {
            Territory t = TerritoryManager.GetTerritory(tile);
            if (t != null)
                return string.Format("{0}:\n{1}\n{2}:\n{3}\n{4}:\n{5}", "Faction".Translate(), Find.FactionManager.AllFactions.FirstOrDefault(f=>f.def == t.factionDef).Name, "Settlement".Translate(),t.settlement.Name, "Influence".Translate(), t.maxInfluence);
            else return "";
        }
        protected override void DoCacheClearing()
        {

            List<MapModeFramework.Region> regions = this.regions;
            int regionsCount = regions.Count;
            for (int i = 0; i < regionsCount; i++)
            {
                MapModeFramework.Region region = regions[i];
                EdgesCache.ClearRegionCache(region);
            }
        }
        public override void SetRegions()
        {
            regions.Clear();
            Log.Message("TerritoryManager测试据点个数:" + TerritoryManager.territorySettlementList.Count);

            foreach (var ts in TerritoryManager.territorySettlementList)
            {
                AddRegions(ts);
            }

        }
        public void AddRegions(TerritorySettlement ts)
        {
            if (ts.factiondef != null)
            {
            
                List<int> tiles =ts.territoryList.Select(t => t.tileId).ToList();

                try
                {
                    MapModeFramework.Region region = new MapModeFramework.Region(ts.settlement.ID.ToString(), tiles, false, FactionMaterials[ts.factiondef.defName], true, FactionBorderMaterials[ts.factiondef.defName]);
                    regions.Add(region);
                }
                catch (Exception ex)
                {
                    Log.Message(ex);

                }
            }
        }
  
    }

}
