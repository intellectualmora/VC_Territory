using MapModeFramework;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using static HarmonyLib.Code;

namespace VC_Territory
{
    public class Gameplay
    {
        public static void TaxReward(TerritorySettlement ts)
        {
            if (ts?.settlement == null || ts.settlement.Faction != Faction.OfPlayer)
                return;

            Map map = ts.settlement.Map;
            if (map == null || !map.IsPlayerHome)
                return; 

            int territoryCount = ts.territoryList.Count;
            if (territoryCount <= 0)
                return;

            int rewardPerTerritory = 1;
            int totalReward = (int)(0.2f*rewardPerTerritory * territoryCount);

            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            silver.stackCount = totalReward;
            List<Thing> things = new List<Thing> { silver };

            DropPodUtility.DropThingsNear(map.Center, map, things);

            Find.LetterStack.ReceiveLetter(
                "TaxReward".Translate(), // 标题
                "TaxRewardMessage".Translate(ts.settlement.Name, totalReward, territoryCount), // 内容
                LetterDefOf.PositiveEvent,
                new GlobalTargetInfo(map.Center, map)
            );
        }

        public static void PlayerInfluenceChange(TerritorySettlement ts) //每变化5000财富值触发一次
        {
            if (ts?.settlement == null || ts.settlement.Faction != Faction.OfPlayer)
                return;

            Map map = ts.settlement.Map;
            if (map == null || !map.IsPlayerHome)
                return;

            int territoryCount = ts.territoryList.Count;
            if (territoryCount <= 0)
                return;

            // 读取当前地图的财富值
            float currentWealth = map.wealthWatcher.WealthTotal;

            // 每 10000 白银对应 1 点影响力
            float influencePerWealth = 1f / 20000f;
            float expectedInfluence = Mathf.Max(9f + currentWealth * influencePerWealth,10f);

            float currentInfluence = ts.Influence;

            // 差值
            float deltaInfluence = expectedInfluence - currentInfluence;

            string message;

            if (deltaInfluence > 0.5f)
            {
                // 增加影响力
                ts.SetInfluence(ts.Influence+deltaInfluence);
                message = "InfluenceGain".Translate((int)currentWealth, deltaInfluence.ToString("F1"));
                Messages.Message(
                   message,
                   new GlobalTargetInfo(map.Center, map),
                   MessageTypeDefOf.PositiveEvent // 或用 NeutralEvent / NegativeEvent 根据实际情况
                );
            }
            else if (deltaInfluence < -0.5f)
            {
                // 减少影响力
                ts.SetInfluence(ts.Influence-deltaInfluence);
                message = "InfluenceLoss".Translate((int)currentWealth, (-deltaInfluence).ToString("F1")); ;
                Messages.Message(
                   message,
                   new GlobalTargetInfo(map.Center, map),
                   MessageTypeDefOf.NegativeEvent
                );
            }
            else
            {
                message = "InfluenceStable".Translate((int)currentWealth);
                Messages.Message(
                   message,
                   new GlobalTargetInfo(map.Center, map),
                   MessageTypeDefOf.NeutralEvent
                );
            }

           
        }

    }



}
