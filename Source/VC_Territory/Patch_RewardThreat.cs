using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VC_Territory
{
    [StaticConstructorOnStartup]
    public static class Patch_RewardThreat
    {
        public static Harmony harmony;
        static Patch_RewardThreat()
        {
            Harmony harmony;
            if ((harmony = Patch_RewardThreat.harmony) == null)
            {
                harmony = (Patch_RewardThreat.harmony = new Harmony("VC_Territory"));
            }
            Patch_RewardThreat.harmony = harmony;
            PatchRewardThreat();
            Log.Message("VCTerritory: Patch Successful");
        }

        public static void PatchRewardThreat()
        {
            MethodInfo original1 = AccessTools.Method(typeof(QuestNode_GiveRewards), "RunInt");
            HarmonyMethod postfix1 = new HarmonyMethod(typeof(Patch_RewardThreat).GetMethod("QuestCompletePatch"));
            harmony.Patch(original1, postfix: postfix1);

        }

        public static void QuestCompletePatch(QuestNode_GiveRewards __instance)
        {
            var quest = QuestGen.quest;
            // Find enemy and asker faction
            var enemy = QuestGen.slate.Get<Faction>("enemyFaction");
            if (enemy != null)
            {
                quest.AddPart(new QuestPart_ChangeInfluence(enemy, -0.1f));
            }
            var asker = QuestGen.slate.Get<Pawn>("asker");
            if (asker != null && asker.Faction != null)
            {
                quest.AddPart(new QuestPart_ChangeInfluence(asker.Faction, 0.1f));
            }
        }


    }
}