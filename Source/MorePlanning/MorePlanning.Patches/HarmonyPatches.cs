using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MorePlanning.Common;
using RimWorld;
using Verse;

namespace MorePlanning.Patches
{
    internal static class HarmonyPatches
    {

        public static void PatchVanilla(Harmony harmony)
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


       //Dubs Mint Menu

        public static void PatchDubsMintMenus(Harmony harmony)
        {
            
                Type mintType = AccessTools.TypeByName("DubsMintMenus.MainTabWindow_MintArchitect");
                if (mintType == null)
                    return;

                //Log.Message("[MorePlanning] Dubs Mint Menus detected, patching.");
                // Only now we are on the main thread, safe to patch
                FieldInfo desPanelsField = AccessTools.Field(mintType, "desPanelsCached");
                if (desPanelsField == null)
                    return;

                MethodInfo transpiler = AccessTools.Method(typeof(HarmonyPatches), nameof(CacheDesPanelsDubs_Transpiler));
                            
                var property = mintType.GetProperty("CacheDesPanels", BindingFlags.Instance | BindingFlags.NonPublic);
                if (property?.GetGetMethod(true) != null)
                {
                    harmony.Patch(property.GetGetMethod(true),
                        transpiler: new HarmonyMethod(transpiler));
                }
            
        }        


       
        public static IEnumerable<CodeInstruction> CacheDesPanelsDubs_Transpiler(IEnumerable<CodeInstruction> instrs)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instrs);

            // Dynamically find the field desPanelsCached
            Type mintType = AccessTools.TypeByName("DubsMintMenus.MainTabWindow_MintArchitect");
            if (mintType == null)
                return codes;

            FieldInfo desPanelsField = AccessTools.Field(mintType, "desPanelsCached");
            if (desPanelsField == null)
                return codes;

            MethodInfo addDesignators = AccessTools.DeclaredMethod(typeof(MorePlanningMod), nameof(MorePlanningMod.AddDesignators));
            if (addDesignators == null)
                return codes;

            // Inject call after the last stfld to desPanelsCached
            for (int i = codes.Count - 1; i >= 0; i--)
            {
                if (codes[i].opcode == OpCodes.Stfld && codes[i].operand is FieldInfo fi && fi == desPanelsField)
                {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, addDesignators));
                    break;
                }
            }

            return codes;
        }


       //vanilla patches

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

        public static void DoPlaySettingsGlobalControls_Postfix(WidgetRow row, bool worldView)
        {
            if (worldView)
                return;

            var toggleable = MorePlanningMod.Instance.PlanningVisibility;
            row.ToggleableIcon(ref toggleable, Resources.IconShowPlanning, "MorePlanning.PlanVisibility".Translate());
            MorePlanningMod.Instance.PlanningVisibility = toggleable;
        }

        
    }
}
