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
    public class InfluenceFactorDef : Def
    {
        public float minInfluence;
        public float maxDistance;
        public float DefaultHillinessFactor;
        public float DefaultBiomeFactor;
        public float DefaultFactionFactor;

        public Dictionary<Hilliness, float> hillinessFactorDict;
        public Dictionary<BiomeDef, float> biomeFactorDict;
        public Dictionary<FactionDef, float> factionFactorDict;
    }
}
