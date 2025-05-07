using System.Diagnostics;
using RimWorld;
using Verse;

namespace VC_Territory
{
    [StaticConstructorOnStartup]

    public static class VC_MyMod_DefInject
    {
        static VC_MyMod_DefInject()
        {
            ThreadChecker.Init();
            DefDatabase<VC_Territory.InfluenceFactorDef>.AddAllInMods();
        }
    }
    public static class ThreadChecker
    {
        private static int mainThreadId;

        public static void Init()
        {
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public static bool IsMainThread => System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId;
    }
}