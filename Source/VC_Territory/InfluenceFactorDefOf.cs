using RimWorld;
using Verse;

namespace VC_Territory
{
    [DefOf]
    public static class InfluenceFactorDefOf
    {
        public static InfluenceFactorDef InfluenceFactor;
        static InfluenceFactorDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(InfluenceFactorDefOf));
    }
}