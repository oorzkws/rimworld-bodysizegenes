using System;
using System.Collections.Generic;
using System.Linq;
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
    [HarmonyPatch(typeof(VariedBodySizes_GameComponent), "GetVariedBodySize", MethodType.Normal)]
    public static class VariedBodySizes_GameComponentPatch
    {
        public static void Postfix(ref float __result, Pawn pawn)
        {
            // GetStatValue will access needs, which if null, will break loading /tableflip
            if (!ModsConfig.BiotechActive || pawn?.RaceProps.Humanlike != true || pawn.needs == null) return;
            // Genes? Y/N
            var bodySizeMultiplier = StatDef.Named("bodySizeFactor");
            if (bodySizeMultiplier != null)
                __result *= pawn.GetStatValue(bodySizeMultiplier);

            // Babies won't fit in cribs if body size exceeds 0.25
            if (pawn.ageTracker.CurLifeStage.developmentalStage <= DevelopmentalStage.Baby)
            {
                __result = Mathf.Min(__result, 0.25f);
            }
        }
    }
    
    [HarmonyPatch]
    public static class VariedBodySizesModSettings_AffectMeleeDamagePatch
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
            if (!__result.VariedBodySizes.ContainsKey("Human") || __result.VariedBodySizes["Human"] == new FloatRange(0.9f, 1.1f))
                __result.VariedBodySizes["Human"] = new FloatRange(0.95f, 1.05f);
        }
        
    }
}