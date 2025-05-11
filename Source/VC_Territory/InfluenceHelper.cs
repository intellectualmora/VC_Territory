using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VC_Territory
{
    public static class InfluenceHelper
    {
        static void AddInfluence(this TerritorySettlement ts, float amount)
        {
            ts.SetInfluence(ts.Influence + amount);
        }

        public static void AddInfluenceToFaction(Faction f, float amount)
        {
            bool condition(Settlement x) => x.Faction == f;
            Log.Message("VC_Territory: Add influence");
            if (Find.WorldObjects.Settlements.Any((Func<Settlement, bool>)condition))
            {
                var rndSettlemnt = Find.WorldObjects.Settlements.Where(condition).RandomElement();
                TerritoryManager.territorySettlementList.FirstOrDefault(x=>x.settlement == rndSettlemnt)?.AddInfluence(amount);

                // Select the influenced settlement
                CameraJumper.TryShowWorld();
                Find.WorldSelector.Select(rndSettlemnt);
                string influenceString = amount > 0f ? "VCT.InfluenceAdded".Translate(f.GetCallLabel(), amount.ToString("0.0")) : "VCT.InfluenceReduced".Translate(f.GetCallLabel(), (-amount).ToString("0.0"));
                Messages.Message(influenceString, MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
