using RimWorld;
using Verse;

namespace VC_Territory
{
    class QuestPart_ChangeInfluence : QuestPart
    {
        private Faction _faction;
        private float _influenceChange;

        public QuestPart_ChangeInfluence(Faction faction, float influenceChange)
        {
            _faction = faction;
            _influenceChange = influenceChange;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            if(this.quest.State != QuestState.EndedFailed)
            InfluenceHelper.AddInfluenceToFaction(_faction, _influenceChange);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref _faction, "faction");
            Scribe_Values.Look(ref _influenceChange, "influenceChange", 0f);
        }
    }
}
