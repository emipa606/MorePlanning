using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DubsMintMenus;
using HarmonyLib;
using MorePlanning.Common;
using RimWorld;
using Verse;

namespace MorePlanning.Patches;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
    static HarmonyPatches()
    {
        var harmony = new Harmony("com.github.alandariva.moreplanning");
        PatchVanilla(harmony);
        PatchDubsMintMenus(harmony);
    }

    private static void PatchDubsMintMenus(Harmony harmony)
    {
        try
        {
            ((Action<Harmony>)delegate(Harmony h)
            {
                var property = typeof(MainTabWindow_MintArchitect).GetProperty("CacheDesPanels",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                var method = typeof(HarmonyPatches).GetMethod(nameof(CacheDesPanelsDubs_Transpiler));
                h.Patch(property?.GetMethod, null, null, new HarmonyMethod(method));
            })(harmony);
        }
        catch (TypeLoadException)
        {
        }
    }

    private static void PatchVanilla(Harmony harmony)
    {
        var method = typeof(Designation).GetMethod(nameof(Designation.ExposeData));
        var method2 = typeof(HarmonyPatches).GetMethod(nameof(DesignationPlanningExposeData_Prefix));
        harmony.Patch(method, new HarmonyMethod(method2));
        var method3 =
            typeof(MainTabWindow_Architect).GetMethod("CacheDesPanels", BindingFlags.Instance | BindingFlags.NonPublic);
        var method4 = typeof(HarmonyPatches).GetMethod(nameof(CacheDesPanels_Postfix));
        harmony.Patch(method3, null, new HarmonyMethod(method4));
        var method5 = typeof(PlaySettings).GetMethod(nameof(PlaySettings.DoPlaySettingsGlobalControls),
            BindingFlags.Instance | BindingFlags.Public);
        var method6 = typeof(HarmonyPatches).GetMethod(nameof(DoPlaySettingsGlobalControls_Postfix));
        harmony.Patch(method5, null, new HarmonyMethod(method6));
    }

    public static bool DesignationPlanningExposeData_Prefix(Designation __instance)
    {
        if (__instance is PlanDesignation planDesignation)
        {
            Scribe_Values.Look(ref planDesignation.Color, "Color", 0, true);
        }

        return true;
    }

    public static void CacheDesPanels_Postfix()
    {
        MorePlanningMod.AddDesignators();
    }

    public static IEnumerable<CodeInstruction> CacheDesPanelsDubs_Transpiler(IEnumerable<CodeInstruction> instrs)
    {
        var fld = typeof(MainTabWindow_MintArchitect).GetField("desPanelsCached",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var addDesignators = typeof(MorePlanningMod).GetMethod(nameof(MorePlanningMod.AddDesignators),
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        var codes = new List<CodeInstruction>(instrs);
        if (fld == null || addDesignators == null)
        {
            return codes;
        }

        // Insert after the final assignment to desPanelsCached
        for (var i = codes.Count - 1; i >= 0; i--)
        {
            if (codes[i].opcode != OpCodes.Stfld || codes[i].operand is not FieldInfo fi || fi != fld)
            {
                continue;
            }

            codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, addDesignators));
            break;
        }

        return codes;
    }

    public static void DoPlaySettingsGlobalControls_Postfix(WidgetRow row, bool worldView)
    {
        if (worldView)
        {
            return;
        }

        var toggleable = MorePlanningMod.Instance.PlanningVisibility;
        row.ToggleableIcon(ref toggleable, Resources.IconShowPlanning, "MorePlanning.PlanVisibility".Translate());
        MorePlanningMod.Instance.PlanningVisibility = toggleable;
    }
}