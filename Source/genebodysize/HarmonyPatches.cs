using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using VariedBodySizes;
using Verse;

// ReSharper disable InconsistentNaming

namespace genebodysize;

public class HarmonyPatches
{
    [HarmonyPatch]
    public static class VariedBodySizesMod_SettingsPatch
    {
        // Since it's internal we need to do a bit of reflection
        static MethodBase TargetMethod()
        {
            var typeByName = AccessTools.TypeByName("VariedBodySizes.VariedBodySizesMod");
            return AccessTools.PropertyGetter(typeByName, "Settings");
        }

        public static void Postfix(ref VariedBodySizesModSettings __result)
        {
            __result.AffectMeleeDamage = true;
            __result.AffectRealBodySize = true;
            __result.AffectRealHealthScale = true;
            __result.AffectMeleeDodgeChance = true;
            // Only override if default
            if (!__result.VariedBodySizes.ContainsKey("Human") ||
                __result.VariedBodySizes["Human"] == new FloatRange(0.9f, 1.1f))
                __result.VariedBodySizes["Human"] = new FloatRange(0.95f, 1.05f);
        }

    }
    
    [HarmonyPatch(typeof(VariedBodySizes_GameComponent), "OnCalculateBodySize")]
    public static class Pawn_BodySizePatch_OnCalculateBodySizePatch
    {
        private static readonly TimedCache<float> statCache = new(360);
        private static StatDef bodySizeMultiplier;
        // Register a modifier for ez access
        public static float Postfix(float result, Pawn pawn)
        {
            // GetStatValue will access needs, which if null, will break loading /tableflip
            if (!ModsConfig.BiotechActive || pawn?.RaceProps.Humanlike != true || pawn.needs == null && !pawn.Dead)
                return result;
            
            // cached value, or calculate/cache/return
            if (statCache.TryGet(pawn, out var pawnSize))
            {
                return pawnSize;
            }
        
            // Load-time fun!
            bodySizeMultiplier ??= StatDef.Named("bodySizeFactor");

            return statCache.SetAndReturn(pawn, result * pawn.GetStatValue(bodySizeMultiplier, cacheStaleAfterTicks: 3600));
        }
        
    }
}