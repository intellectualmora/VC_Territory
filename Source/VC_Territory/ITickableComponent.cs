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
    public interface ITickableComponent : IExposable
    {
        void LongTermTick();
    }
}
