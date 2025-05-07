using MapModeFramework;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace VC_Territory
{
    public class Territory : IExposable
    {
        public int tileId;
        public Settlement settlement;
        public float maxInfluence;
        public FactionDef factionDef => settlement.Faction.def;
        public Dictionary<int, float>  influencesDict = new Dictionary<int, float>();

        private List<int> _keys;
        private List<float> _values;
        private int _settlementID;

        public List<IExposable> extraComponents = new List<IExposable>();

        public void Update_LongTerm_Tick()
        {
            foreach (var comp in extraComponents)
            {
                if (comp is ITickableComponent tickable)
                {
                    tickable.LongTermTick();
                }
            }
        }

        private void _Broadcast(Settlement settle, float newInfluence)
        {
            float distance = Find.WorldGrid.ApproxDistanceInTiles(settle.Tile, tileId);
            List<int> outNeighbors = new List<int>();
            Find.WorldGrid.GetTileNeighbors(tileId, outNeighbors);
            if (newInfluence * Mathf.Max((TerritoryManager.maxDistance - distance) / TerritoryManager.maxDistance, 0f) > TerritoryManager.minInfluence)
                foreach (var neig in outNeighbors)
                {
                    if (!Find.World.grid[neig].WaterCovered)
                    {
                        Territory t;
                        if (!TerritoryManager.HasTerritory(neig))
                        {
                            t = new Territory();
                            t.tileId = neig;
                            TerritoryManager.AddTerritory(t);
                        }
                        else
                        {
                            t = TerritoryManager.GetTerritory(neig);
                        }
                        t.Notify_Influence_Add(settle, newInfluence);
                    }
                }
        }
        public void Notify_Influence_Add(Settlement settle, float Influence)
        {
            WorldGrid grid = Find.WorldGrid;
            Hilliness hill = grid[tileId].hilliness;
            BiomeDef biome = grid[tileId].biome;
            float distance = Find.WorldGrid.ApproxDistanceInTiles(settle.Tile, tileId);
            float hf = InfluenceFactorDefOf.InfluenceFactor.hillinessFactorDict.ContainsKey(hill) ? InfluenceFactorDefOf.InfluenceFactor.hillinessFactorDict[hill] : TerritoryManager.DefaultHillinessFactor;
            float bf = InfluenceFactorDefOf.InfluenceFactor.biomeFactorDict.ContainsKey(biome) ? InfluenceFactorDefOf.InfluenceFactor.biomeFactorDict[biome] : TerritoryManager.DefaultBiomeFactor;
            float newInfluence = Influence  * hf * bf;

            if (influencesDict.ContainsKey(settle.ID))
            {
                if (influencesDict[settle.ID] < newInfluence)
                {
                    influencesDict[settle.ID] = newInfluence;
                }
                return;
            }
            this.influencesDict.Add(settle.ID, newInfluence);
            _Broadcast(settle,newInfluence);
        }
        public void Notify_Influence_Remove(Settlement settle)
        {
            influencesDict.Remove(settle.ID);
        }
        public void Notify_Influence_Change(Settlement settle,float Influence)
        {
            influencesDict[settle.ID] = Influence;
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref tileId, "VC_tileId");
            Scribe_Values.Look(ref maxInfluence, "VC_maxInfluence");
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                _keys = new List<int>(influencesDict.Keys);
                _values = new List<float>(influencesDict.Values);
                _settlementID = settlement.ID;
            }

            // Scribe the keys and values
            Scribe_Collections.Look(ref _keys, "VC_influencesDict_keys", LookMode.Value);
            Scribe_Collections.Look(ref _values, "VC_influencesDict_values", LookMode.Value);
            Scribe_Values.Look(ref _settlementID, "VC_settlementID");
            Scribe_Collections.Look(ref extraComponents, "VC_TerritoryExtraComponents", LookMode.Deep);

            // Load
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                settlement = Find.WorldObjects.Settlements.Find(settle => settle.ID == _settlementID);
                influencesDict = new Dictionary<int, float>();
                if (_keys != null && _values != null)
                {
                    for (int i = 0; i < _keys.Count && i < _values.Count; i++)
                    {
                            influencesDict[_keys[i]] = _values[i];
                    }
                }
            }
            
        }
    }
   
    

}
