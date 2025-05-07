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
    public class TerritorySettlement : IExposable
    {
        public Settlement settlement;
        public List<Territory> territoryList => TerritoryManager.GetTerritoriesBySettlement(settlement);
        private float influence;
        public float Influence => influence;
        public FactionDef factiondef => settlement.Faction.def;
        private int _settlementId;
        public bool isDirty = false;


        public List<IExposable> extraComponents = new List<IExposable>();

        public TerritorySettlement()
        {

        }
        public void SetInfluence(float inf)
        {
            influence = inf;
            isDirty = true;
        }
        public void Update_LongTerm_Tick()
        {
            foreach (var comp in extraComponents)
            {
                if (comp is ITickableComponent tickable)
                {
                    tickable.LongTermTick();
                }
            }
            if (isDirty)
            {
                TerritoryManager.Notify_Influence_Change(this); //提示影响力变化
                isDirty = false;
            }
        }
        public TerritorySettlement(Settlement settle,float inf)
        {
            this.settlement = settle;
            this.influence = inf;
        }
        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                _settlementId = settlement.ID;
            }
            Scribe_Values.Look(ref influence, "VC_influence");
            Scribe_Values.Look(ref _settlementId, "VC_territorySettlementId");
            Scribe_Values.Look(ref isDirty, "VC_TSisDirty");
            Scribe_Collections.Look(ref extraComponents, "VC_TerritorySettlementExtraComponents", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                settlement = Find.WorldObjects.Settlements.Find(s => s.ID == _settlementId);
            }
        }
    
        
    }



}
