﻿using RimWorld;
using Verse;
using UnityEngine;
using System;
using RimWorld.Planet;
using MapModeFramework;
using System.Collections.Generic;
using System.Linq;
namespace VC_Territory
{
    public class VC_TerritorySetting : ModSettings
    {
        public static bool isInit = false;
        public Vector2 scrollPosition = Vector2.zero;
        public Vector2 scrollPosition2 = Vector2.zero;
        public Vector2 scrollPosition3 = Vector2.zero;
        public Vector2 scrollPosition4 = Vector2.zero;

        private Vector2 outerScrollPos = Vector2.zero;
        public static float minInfluence = 0.1f;
        public static float maxDistance = 100f;
        public static Dictionary<Hilliness, float> hillinessFactorDict;
        public static Dictionary<BiomeDef, float> biomeFactorDict;
        public static Dictionary<FactionDef, Color> factionColorDict;

        public static bool triggerTax = true;
        public static bool triggerInfluenceChange = true;


        private static List<Hilliness> _hillinessFactorDict_keys;
        private static List<float> _hillinessFactorDict_values;
        private static List<string> _biomeFactorDict_keys;
        private static List<float> _biomeFactorDict_values;
        private static List<string> _factionColorDict_keys;
        private static List<float> _factionColorDict_values_r;
        private static List<float> _factionColorDict_values_g;
        private static List<float> _factionColorDict_values_b;

        private static readonly List<Color> presetColors = new List<Color>
        {
            // 基础色
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            Color.white,
            Color.black,
            Color.gray,

            // 深/亮调
            new Color(0.8f, 0.2f, 0.2f),      // 深红
            new Color(0.2f, 0.8f, 0.2f),      // 草绿
            new Color(0.2f, 0.2f, 0.8f),      // 深蓝
            new Color(1f, 1f, 0.6f),          // 柠檬黄
            new Color(0.5f, 1f, 1f),          // 浅青
            new Color(1f, 0.5f, 1f),          // 粉紫
            new Color(0.8f, 0.8f, 0.8f),      // 浅灰
            new Color(0.4f, 0.4f, 0.4f),      // 深灰

            // 棕/橙/金属色
            new Color(0.5f, 0.25f, 0f),       // 棕色
            new Color(1f, 0.6f, 0.2f),        // 橙色
            new Color(1f, 0.84f, 0f),         // 金色
            new Color(0.75f, 0.75f, 0.75f),   // 银色

            // 粉色系
            new Color(1f, 0.7f, 0.8f),        // 樱花粉
            new Color(1f, 0.4f, 0.7f),        // 粉红
            new Color(1f, 0.6f, 0.6f),        // 肤色

            // 蓝绿色系
            new Color(0.2f, 0.6f, 0.6f),      // 深青
            new Color(0f, 0.5f, 0.5f),        // 孔雀蓝
            new Color(0.4f, 0.7f, 0.7f),      // 浅青灰

            // 紫色系
            new Color(0.5f, 0f, 1f),          // 紫罗兰
            new Color(0.6f, 0.4f, 1f),        // 浅紫
            new Color(0.4f, 0.2f, 0.6f),      // 暗紫

            // 绿色系
            new Color(0.4f, 0.6f, 0.2f),      // 橄榄绿
            new Color(0.3f, 0.5f, 0.1f),      // 军绿
            new Color(0.6f, 1f, 0.6f),        // 浅绿

            // 蓝色系
            new Color(0f, 0f, 0.5f),          // 海军蓝
            new Color(0.2f, 0.4f, 1f),        // 电蓝
            new Color(0.6f, 0.8f, 1f),        // 天空蓝

            // 补充
            new Color(0.2f, 0.2f, 0.2f),      // 几乎黑
            new Color(1f, 0.8f, 0.4f),        // 奶油黄
            new Color(0.4f, 0.3f, 0.2f),      // 木头棕
            new Color(0.6f, 0.3f, 0.2f),      // 烧砖红
        };

        private Texture2D SolidColorTexture(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref minInfluence, "VC_minInfluence", 0.1f);
            Scribe_Values.Look(ref maxDistance, "VC_maxDistance", 100f);

            Scribe_Values.Look(ref triggerTax, "VC_triggerTax");
            Scribe_Values.Look(ref triggerInfluenceChange, "VC_triggerInfluenceChange");

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                _hillinessFactorDict_keys = new List<Hilliness>(hillinessFactorDict.Keys);
                _hillinessFactorDict_values = new List<float>(hillinessFactorDict.Values);
                _biomeFactorDict_keys = new List<string>(biomeFactorDict.Keys.Select(b=>b.defName).ToList());
                _biomeFactorDict_values = new List<float>(biomeFactorDict.Values);
                _factionColorDict_keys = new List<string>(DefDatabase<FactionDef>.AllDefs.Select(f => f.defName).ToList());
                _factionColorDict_values_r = new List<float>(Materials.matsTerritory.Select(c=>c.Value.color.r).ToList());
                _factionColorDict_values_g = new List<float>(Materials.matsTerritory.Select(c => c.Value.color.g).ToList());
                _factionColorDict_values_b = new List<float>(Materials.matsTerritory.Select(c => c.Value.color.b).ToList());
            }
            Scribe_Collections.Look(ref _hillinessFactorDict_keys, "VC_hillinessFactorDict_keys", LookMode.Value);
            Scribe_Collections.Look(ref _hillinessFactorDict_values, "VC_hillinessFactorDict_values", LookMode.Value);
            Scribe_Collections.Look(ref _biomeFactorDict_keys, "VC_biomeFactorDict_keys", LookMode.Value);
            Scribe_Collections.Look(ref _biomeFactorDict_values, "VC_biomeFactorDict_values", LookMode.Value);

            Scribe_Collections.Look(ref _factionColorDict_keys, "VC_factionColorDict_keys", LookMode.Value);
            Scribe_Collections.Look(ref _factionColorDict_values_r, "VC_factionColorDict_values_r", LookMode.Value);
            Scribe_Collections.Look(ref _factionColorDict_values_g, "VC_factionColorDict_values_g", LookMode.Value);
            Scribe_Collections.Look(ref _factionColorDict_values_b, "VC_factionColorDict_values_b", LookMode.Value);
        }
        public static void Init()
        {
            Log.Message("Initalize Setting");

            hillinessFactorDict = new Dictionary<Hilliness, float>();
            if (_hillinessFactorDict_keys != null && _hillinessFactorDict_values != null)
            {
                for (int i = 0; i < _hillinessFactorDict_keys.Count && i < _hillinessFactorDict_values.Count; i++)
                {
                    hillinessFactorDict[_hillinessFactorDict_keys[i]] = _hillinessFactorDict_values[i];
                }
            }

            biomeFactorDict = new Dictionary<BiomeDef, float>();
            if (_biomeFactorDict_keys != null && _biomeFactorDict_values != null)
            {
                for (int i = 0; i < _biomeFactorDict_keys.Count && i < _biomeFactorDict_values.Count; i++)
                {
                    if (DefDatabase<BiomeDef>.AllDefs.Any(b => b.defName == _biomeFactorDict_keys[i]))
                        biomeFactorDict[DefDatabase<BiomeDef>.AllDefs.FirstOrDefault(b => b.defName == _biomeFactorDict_keys[i])] = _biomeFactorDict_values[i];
                }
            }

            factionColorDict = new Dictionary<FactionDef, Color>();
            if (_factionColorDict_keys != null && _factionColorDict_values_r != null && _factionColorDict_values_g != null && _factionColorDict_values_b != null)
            {
                for (int i = 0; i < _factionColorDict_keys.Count && i < _factionColorDict_values_r.Count; i++)
                {
                    if (DefDatabase<FactionDef>.AllDefs.Any(b => b.defName == _factionColorDict_keys[i]))
                        factionColorDict[DefDatabase<FactionDef>.AllDefs.FirstOrDefault(b => b.defName == _factionColorDict_keys[i])] = new Color(_factionColorDict_values_r[i], _factionColorDict_values_g[i], _factionColorDict_values_b[i]);
                }
            }

            isInit = true;
        }
        public void DoWindowContents(Rect inRect)
        {
            float contentHeight = 1000f; 
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, contentHeight); 

            Widgets.BeginScrollView(inRect, ref outerScrollPos, viewRect);


            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect); 
            listing.Gap(24f); 
            listing.Label("TerritoryDebug".Translate());
            if (listing.ButtonText("Reset".Translate(),widthPct:0.2f))
            {
                try
                {
                    Materials.Init();
                    TerritoryManager.InfluenceInit(); // 你自己的初始化函数
                    Messages.Message("TerritorySuccessfullyInitialized".Translate(), MessageTypeDefOf.PositiveEvent, false);
                }
                catch (Exception ex)
                {
                    Log.Error($"TerritoryFailInitialized: {ex}");
                    Messages.Message("TerritoryFailInitializedDetail".Translate(), MessageTypeDefOf.RejectInput, false);
                }
            }
            if (listing.ButtonText("TestMatrials".Translate(), widthPct: 0.2f))
            {
                    foreach (var ts in TerritoryManager.territorySettlementList)
                    {
                        List<int> tiles = ts.territoryList.Select(t => t.tileId).ToList();

                        try
                        {
                            MapModeFramework.Region region = new MapModeFramework.Region(ts.settlement.ID.ToString(), tiles, false, Materials.matsTerritory[ts.factiondef.defName], true, Materials.matsBorderTerritory[ts.factiondef.defName]);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"MatrialFailInitialized: {ex}");
                            Messages.Message("MatrialFailInitializedDetail".Translate(), MessageTypeDefOf.RejectInput, false);
                            break;
                        }
                    Messages.Message("MatrialSuccessfullyInitialized".Translate(), MessageTypeDefOf.PositiveEvent, false);

                }
            }
            if (listing.ButtonText("TestSettlement".Translate(), widthPct: 0.2f))
            {
                if (TerritoryManager.territories.Count <= 0)
                {
                    Messages.Message("TerritoriesError".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }
                if (TerritoryManager.territorySettlementList.Count <= 0)
                {
                    Messages.Message("TerritorySettlementError".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }
                Messages.Message("TerritorySettlementListNum".Translate() + TerritoryManager.territorySettlementList.Count, MessageTypeDefOf.PositiveEvent, false);
                foreach (var ts in TerritoryManager.territorySettlementList)
                {
                    List<int> tiles = ts.territoryList.Select(t => t.tileId).ToList();
                    if (tiles.Count <= 0)
                    {
                        Messages.Message("ZeroTiles".Translate(), MessageTypeDefOf.RejectInput, false);
                        break;
                    }
                }
            }
            listing.GapLine();

            listing.CheckboxLabeled("TriggerTax".Translate(), ref triggerTax, "TriggerTaxTooltip".Translate());
            listing.CheckboxLabeled("TriggerInfluenceChange".Translate(), ref triggerInfluenceChange, "TriggerInfluenceChangeTooltip".Translate());

            listing.GapLine();


            listing.Label("MinInfluence".Translate());
            if (minInfluence <= 0)
            {
                minInfluence = InfluenceFactorDefOf.InfluenceFactor.minInfluence;
            }
            if (maxDistance <= 0)
            {
                maxDistance = InfluenceFactorDefOf.InfluenceFactor.maxDistance;
            }
            minInfluence = Widgets.HorizontalSlider(new Rect(300, listing.CurHeight - 25, 200f, 30f),
            minInfluence, 0.01f, 1f, true, minInfluence.ToString(), roundTo: 0.01f);
            listing.GapLine(); 

            listing.Label("MaxDistance".Translate());
            maxDistance = Widgets.HorizontalSlider(new Rect(300, listing.CurHeight - 25, 200f, 30f),
            maxDistance, 0f, 200f, true, maxDistance.ToString(), roundTo: 1f);
            listing.GapLine();

            listing.Label("HillinessFactorList".Translate());
            float viewHeight = 200f;
            List<Hilliness> hills = Enum.GetValues(typeof(Hilliness)).Cast<Hilliness>().ToList();
            float scrollViewInnerHeight2 = hills.Count * 32f + 10f;
            Rect scrollOutRect2 = listing.GetRect(viewHeight);
            Rect scrollViewRect2 = new Rect(0, 0, scrollOutRect2.width - 32f, scrollViewInnerHeight2);
            Widgets.BeginScrollView(scrollOutRect2, ref scrollPosition2, scrollViewRect2);
            float curY = 0f;
            foreach (var hill in hills)
            {
                if (!hillinessFactorDict.ContainsKey(hill))
                {
                    hillinessFactorDict[hill] = InfluenceFactorDefOf.InfluenceFactor.hillinessFactorDict.ContainsKey(hill)? InfluenceFactorDefOf.InfluenceFactor.hillinessFactorDict[hill]: InfluenceFactorDefOf.InfluenceFactor.DefaultHillinessFactor;
                }
               
                Rect rowRect = new Rect(0f, curY, scrollViewRect2.width, 32f);
                Rect labelRect = new Rect(rowRect.x, rowRect.y, rowRect.width * 0.4f, 32f);
                Rect sliderRect = new Rect(labelRect.xMax + 10f, rowRect.y + 6f, rowRect.width * 0.5f, 20f);

                Widgets.Label(labelRect, hill.ToString());
                hillinessFactorDict[hill] = Widgets.HorizontalSlider(
                    sliderRect,
                    hillinessFactorDict[hill],
                    0f, 1f,
                    middleAlignment: false,
                    label: hillinessFactorDict[hill].ToString("F1"),
                    roundTo: 0.1f
                );

                curY += 32f;
            }
            Widgets.EndScrollView();
            listing.GapLine();


            listing.Label("BiomeFactorList".Translate());
            float scrollViewInnerHeight = DefDatabase<BiomeDef>.AllDefs.Count() * 24f + 10f; // 每个项约占24像素
            Rect scrollOutRect = listing.GetRect(viewHeight); // 分配区域
            Rect scrollViewRect = new Rect(0, 0, scrollOutRect.width - 32f, scrollViewInnerHeight); 
            Widgets.BeginScrollView(scrollOutRect, ref scrollPosition, scrollViewRect);
            curY = 0f;
            foreach (var biome in DefDatabase<BiomeDef>.AllDefs)
            {
                Widgets.Label(new Rect(0f, curY, scrollViewRect.width, 24f), biome.defName);
                if (!biomeFactorDict.ContainsKey(biome))
                {
                    biomeFactorDict[biome] = InfluenceFactorDefOf.InfluenceFactor.biomeFactorDict.ContainsKey(biome) ? InfluenceFactorDefOf.InfluenceFactor.biomeFactorDict[biome] : InfluenceFactorDefOf.InfluenceFactor.DefaultBiomeFactor;
                }

                Rect rowRect = new Rect(0f, curY, scrollViewRect.width, 32f);
                Rect labelRect = new Rect(rowRect.x, rowRect.y, rowRect.width * 0.4f, 32f);
                Rect sliderRect = new Rect(labelRect.xMax + 10f, rowRect.y + 6f, rowRect.width * 0.5f, 20f);

                Widgets.Label(labelRect, biome.defName);

                biomeFactorDict[biome] = Widgets.HorizontalSlider(
                    sliderRect,
                   biomeFactorDict[biome],
                    0f, 1f,
                    middleAlignment: false,
                    label: biomeFactorDict[biome].ToString(),
                    roundTo: 0.1f
                );
                curY += 32f;
            }
            Widgets.EndScrollView();
            listing.GapLine();

            listing.Label("FactionColorList".Translate());
            float scrollViewInnerHeight3 = DefDatabase<FactionDef>.AllDefs.Count() * 24f + 10f; // 每个项约占24像素
            Rect scrollOutRect3 = listing.GetRect(viewHeight); // 分配区域
            Rect scrollViewRect3 = new Rect(0, 0, scrollOutRect3.width - 32f, scrollViewInnerHeight3);
            Widgets.BeginScrollView(scrollOutRect3, ref scrollPosition3, scrollViewRect3);
            curY = 0f;
            foreach (var faction in DefDatabase<FactionDef>.AllDefs)
            {
                Widgets.Label(new Rect(0f, curY, scrollViewRect3.width, 24f), faction.defName);
                if (!factionColorDict.ContainsKey(faction))
                {
                    factionColorDict[faction] = faction.DefaultColor;
                }
                Color color = factionColorDict[faction];


                Rect dropdownRect = new Rect(150, curY, scrollViewRect3.width, 24f);

                Widgets.Dropdown<FactionDef, Color>(
                    dropdownRect,
                    faction,
                    f => factionColorDict[f],
                    f => presetColors.Select(c =>
                    new Widgets.DropdownMenuElement<Color>
                    {
                        option = new FloatMenuOption(
                            label: ColorUtility.ToHtmlStringRGB(c), 
                            action: () =>
                            {
                                factionColorDict[f] = c;
                                Materials.Init();
                                try
                                {
                                    MapModeUI mapModeUI = Find.WindowStack.WindowOfType<MapModeUI>();
                                    if (mapModeUI != null && mapModeUI.CurrentMapMode is MapMode_Territory mt)
                                    {
                                        MapModeComponent.Instance.RequestMapModeSwitch(mt);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error("领土窗口渲染打开错误");
                                }
                            },
                            extraPartWidth: 24f,
                            extraPartOnGUI: rect =>
                            {
                               
                                Widgets.DrawBoxSolid(rect, c);
                                return false; 
                            }
                        ),
                        payload = c
                    }),
                    buttonLabel: "显示当前颜色", 
                    buttonIcon: SolidColorTexture(factionColorDict[faction])
                );


                curY += 32f;
            }
            Widgets.EndScrollView();
            listing.GapLine();



            listing.End();
            Widgets.EndScrollView();
        }
    }

    public class VC_TerritoryMod : Mod
    {
        public static VC_TerritorySetting Settings;

        public VC_TerritoryMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<VC_TerritorySetting>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "MoraTerritory".Translate(); 
        }
    }
}