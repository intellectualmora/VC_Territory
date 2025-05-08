using MapModeFramework;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace VC_Territory
{
    public class TerritoryManager : GameComponent
    {
        public static List<Territory> territories = new List<Territory>();
        public static float minInfluence = InfluenceFactorDefOf.InfluenceFactor.minInfluence;
        public static float maxDistance = InfluenceFactorDefOf.InfluenceFactor.maxDistance;
        public static float DefaultHillinessFactor = InfluenceFactorDefOf.InfluenceFactor.DefaultHillinessFactor;
        public static float DefaultBiomeFactor = InfluenceFactorDefOf.InfluenceFactor.DefaultBiomeFactor;
        public static float DefaultFactionFactor = InfluenceFactorDefOf.InfluenceFactor.DefaultFactionFactor;
        public static List<TerritorySettlement> territorySettlementList = new List<TerritorySettlement>();
        private int tickCounter = 0;
        private const int LongTermTickInterval = 60000; //一天结算一次

        public List<IExposable> extraComponents = new List<IExposable>();


        public TerritoryManager(Game game)
        {
        }

        public static void SelectSettlement(Territory t)
        {
            float maxInfluence = -1;
            foreach (var key in t.influencesDict.Keys)
            {
                if (t.influencesDict[key] > maxInfluence)
                {
                    maxInfluence = t.influencesDict[key];
                    t.settlement = Find.WorldObjects.Settlements.Find(settle => settle.ID == key);
                    t.maxInfluence = maxInfluence;
                }
            }

            if (maxInfluence < 0)
            {
                RemoveTerritory(t);
            }
        }
        public static void InfluenceInitize()
        {
            territories.Clear();
            territorySettlementList.Clear();
            foreach (var settle in Find.WorldObjects.Settlements)
            {
                if (settle.Faction != null)
                {
                    Notify_Influence_Add(settle);
                }
            }
        }
        public static void Notify_Influence_Add(Settlement settle)
        {
            if (territorySettlementList.Any(s => s.settlement == settle))
            {
                return;
            }
            TerritorySettlement territorySettlement = new TerritorySettlement(settle, InfluenceFactorDefOf.InfluenceFactor.factionFactorDict.ContainsKey(settle.Faction.def) ? InfluenceFactorDefOf.InfluenceFactor.factionFactorDict[settle.Faction.def] : DefaultFactionFactor);
            territorySettlementList.Add(territorySettlement);
            Territory t;
            if (TerritoryManager.HasTerritory(settle.Tile))
            {
                t = GetTerritory(settle.Tile);
            }
            else
            {
                t = new Territory();
                t.tileId = territorySettlement.settlement.Tile;
                AddTerritory(t);
            }
            
            t.Notify_Influence_Add(territorySettlement.settlement, territorySettlement.Influence);

            foreach (var it in territories.FindAll(et => et.influencesDict.ContainsKey(settle.ID)))
            {
                SelectSettlement(it);
            }
        }
        public static void Notify_Influence_Remove(Settlement settle)
        {
            if (!territorySettlementList.Any(s => s.settlement == settle))
            {
                return;
            }
            territorySettlementList.Remove(territorySettlementList.Find(s => s.settlement == settle));
            foreach (var it in territories.FindAll(t=>t.influencesDict.ContainsKey(settle.ID)))
            {
                it.Notify_Influence_Remove(settle);
                
            }
            foreach (var it in GetTerritoriesBySettlement(settle))
            {
                SelectSettlement(it);
            }
           
        }
        public static void Notify_Influence_Change(TerritorySettlement ts)
        {
            if (!territorySettlementList.Contains(ts))
            {
                return;
            }
            foreach (var it in territories.FindAll(t => t.influencesDict.ContainsKey(ts.settlement.ID)))
            {
                it.Notify_Influence_Change(ts.settlement,ts.Influence);
            }
            foreach (var it in territories.FindAll(t => t.influencesDict.ContainsKey(ts.settlement.ID)))
            {
                SelectSettlement(it);
            }

        }
        public static Territory AddTerritory(Territory t)
        {
            territories.Add(t);
            return t;
        }
        public static bool HasTerritory(int tileId)
        {
            return territories.Find(t => t.tileId == tileId) != null;
        }
        public static bool HasTerritory(Territory t)
        {
            return territories.Contains(t);
        }
        public static Territory GetTerritory(int tileId)
        {
            return territories.Find(t => t.tileId == tileId);
        }
        public static void RemoveTerritory(Territory t)
        {
            territories.Remove(t);
        }
        public static List<Territory> GetTerritoriesByFactionDef(FactionDef faction)
        {
            return territories.FindAll(t => t.factionDef == faction);
        }
        public static List<Territory> GetTerritoriesBySettlement(Settlement settlement)
        {
            return territories.FindAll(t => t.settlement == settlement);
        }
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            tickCounter++; 
            if (tickCounter >= LongTermTickInterval)
            {
                tickCounter = 0;
                TerritoryManagerLongTermTick();
            }
        }

        public void TerritoryManagerLongTermTick()
        {
            foreach (var comp in extraComponents)
            {
                if (comp is ITickableComponent tickable)
                {
                    tickable.LongTermTick();
                }
            }
            foreach (var ts in territorySettlementList)
            {
                ts.Update_LongTerm_Tick();
            }
            foreach (var t in territories)
            {
                t.Update_LongTerm_Tick();
            }
        }
        public override void ExposeData()
        {

            Scribe_Collections.Look(ref territories, "VC_territories", LookMode.Deep);
            Scribe_Collections.Look(ref territorySettlementList, "VC_territorySettlementList", LookMode.Deep);
            Scribe_Collections.Look(ref extraComponents, "VC_TerritoryManagerExtraComponents", LookMode.Deep);


        }
    }
   
    

}
