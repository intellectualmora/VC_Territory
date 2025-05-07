using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
namespace VC_Territory
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmony = new Harmony("VC_Territory");
            harmony.PatchAll();
        }
    }

}
