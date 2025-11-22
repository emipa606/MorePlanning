using HarmonyLib;
using Mlie;
using MorePlanning.Designators;
using MorePlanning.Patches;
using MorePlanning.Plan;
using MorePlanning.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using Verse;

namespace MorePlanning;

public class MorePlanningMod : Mod
{
    public const string Identifier = "com.github.alandariva.moreplanning";

    private static MorePlanningMod _instance;
    private static string currentVersion;

    // cache pointer to the world component (replaces UtilityWorldObject)
    private MorePlanningWorldComponent _worldComponent;
    public int SelectedColor;

    public MorePlanningModSettings Settings;

    public MorePlanningMod(ModContentPack content) : base(content)
    {
        _instance = this;
        Settings = GetSettings<MorePlanningModSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);

        var harmony = new Harmony("com.github.alandariva.moreplanning");
        HarmonyPatches.PatchVanilla(harmony);

        importOldHugsLibSettings();

        // Do what DefsLoaded used to do, but wait until defs are actually available.
        LongEventHandler.ExecuteWhenFinished(
            () =>
            {
                try
                {
                    InitializeDefsLoadedLikeBehavior();
                } catch(Exception ex)
                {
                    Log.Error($"[{Identifier}] Exception during initialization: {ex}");
                }

                HarmonyPatches.PatchDubsMintMenus(harmony);
            });
    }

    private void EnsureWorldComponentLoaded()
    {
        if(_worldComponent != null)
        {
            return;
        }

        if(Find.World == null)
        {
            return;
        }

        _worldComponent = Find.World.GetComponent<MorePlanningWorldComponent>();
        if(_worldComponent == null)
        {
            // Add if missing (shouldn't normally be necessary if component listed in About.xml)
            Find.World.components.Add(new MorePlanningWorldComponent(Find.World));
            _worldComponent = Find.World.GetComponent<MorePlanningWorldComponent>();
        }

        // trigger relevant UI updates when world is loaded (approx replacement for WorldLoaded)
        MenuUtility.GetPlanningDesignator<VisibilityCommand>()?.SelectedUpdate();
        MenuUtility.GetPlanningDesignator<OpacityCommand>()?.SelectedUpdate();
    }


    private static void importOldHugsLibSettings()
    {
        var hugsLibConfig = Path.Combine(GenFilePaths.SaveDataFolderPath, "HugsLib", "ModSettings.xml");
        if(!new FileInfo(hugsLibConfig).Exists)
        {
            return;
        }

        var xml = XDocument.Load(hugsLibConfig);
        var modNodeName = "com.github.alandariva.moreplanning";

        var modSettings = xml.Root?.Element(modNodeName);
        if(modSettings == null)
        {
            return;
        }

        foreach(var modSetting in modSettings.Elements())
        {
            if(modSetting.Name == "_removeIfBuildingDespawned")
            {
                Instance.Settings.RemoveIfBuildingDespawned = bool.Parse(modSetting.Value);
            }
            if(modSetting.Name == "_shiftKeyForOverride")
            {
                Instance.Settings.ShiftKeyForOverride = bool.Parse(modSetting.Value);
            }
            if(modSetting.Name == "_planOpacity")
            {
                Instance.Settings.PlanOpacity = int.Parse(modSetting.Value);
            }
        }

        Instance.Settings.Write();
        xml.Root.Element(modNodeName)?.Remove();
        xml.Save(hugsLibConfig);

        Log.Message($"[{modNodeName}]: Imported old HugLib-settings");
    }

    private void InitializeDefsLoadedLikeBehavior()
    {
        // This is the rough equivalent of DefsLoaded() in HugsLib
        MorePlanning.Common.Resources.PlanDesignationDef = DefDatabase<DesignationDef>.GetNamedSilentFail("MP_Plan");
        if(MorePlanning.Common.Resources.PlanDesignationDef == null)
        {
            Log.Warning($"[{Identifier}] DesignationDef 'MP_Plan' not found.");
        }

        PlanColorManager.LoadFromSettings();

        // Apply any settings-derived changes to defs (removeIfBuildingDespawned etc)
        UpdatePlanningDefsSetting();

        // Add the color select designators into the "Planning" category if missing
        AddDesignators();
    }

    private void UpdatePlanningDefsSetting()
    {
        var def = DefDatabase<DesignationDef>.GetNamedSilentFail("MP_Plan");
        if(def != null)
        {
            def.removeIfBuildingDespawned = Settings.RemoveIfBuildingDespawned;
        }
    }


    public static void AddDesignators()
    {
        // Get the Planning designation category, fail silently if missing
        var planningCategory = DefDatabase<DesignationCategoryDef>.GetNamedSilentFail("Planning");
        if(planningCategory == null)
        {
            Log.Error("[MorePlanning] Planning designation category not found.");
            return;
        }

        // Get the private list of designators via reflection
        var listField = typeof(DesignationCategoryDef)
                .GetField("resolvedDesignators", BindingFlags.Instance | BindingFlags.NonPublic);

        if(listField == null)
        {
            Log.Error("[MorePlanning] Could not find 'resolvedDesignators' field via reflection.");
            return;
        }

        if(!(listField.GetValue(planningCategory) is List<Designator> list))
        {
            return;
        }

        // Check if the SelectColorDesignator instances already exist
        bool anyExisting = list.OfType<SelectColorDesignator>().Any();
        if(anyExisting)
        {
            return; // already added, nothing to do
        }

        // Add 10 SelectColorDesignator instances
        for(int i = 0; i < PlanColorManager.NumPlans; i++)
        {
            list.Add(new SelectColorDesignator(i));
        }

        //Log.Message("[MorePlanning] Added SelectColorDesignators.");
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        // Simple settings UI similar to what HugsLib exposed. You can refine layout as desired.
        var listing = new Listing_Standard();
        listing.Begin(inRect);

        listing.CheckboxLabeled(
            "MorePlanning.SettingRemoveIfBuildingDespawned.label".Translate(),
            ref Settings.RemoveIfBuildingDespawned,
            "MorePlanning.SettingRemoveIfBuildingDespawned.desc".Translate());

        listing.CheckboxLabeled(
            "MorePlanning.SettingShiftKeyForOverride.label".Translate(),
            ref Settings.ShiftKeyForOverride,
            "MorePlanning.SettingShiftKeyForOverride.desc".Translate());

        listing.Label("MorePlanning.SettingPlanOpacity.label".Translate() + $": {Settings.PlanOpacity}");
        Settings.PlanOpacity = (int)listing.Slider(Settings.PlanOpacity, 0, 100);

        listing.Gap(8f);

        if(listing.ButtonText("MorePlanning.Reset".Translate()))
        {
            Settings.ExposeData_ResetToDefaults();
        }

        if(currentVersion != null)
        {
            listing.Gap();
            GUI.contentColor = Color.gray;
            listing.Label("MorePlanning.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing.End();
    }

    public static void LogError(string text)
    {
        // keep compatibility with existing code calling MorePlanningMod.LogError(...)
        Log.Error($"[{Identifier}] {text}");
    }

    public static void LogMessage(string text) { Log.Message($"[{Identifier}] {text}"); }

    public override string SettingsCategory()
    {
        // shown in Mods menu
        return "MorePlanning_ModName".Translate();
    }

    public void SettingsChanged()
    {
        UpdatePlanningDefsSetting();
        PlanColorManager.InvalidateColors();
    }

    public override void WriteSettings()
    {
        // called by the vanilla UI once the user closes/accepts the settings window
        base.WriteSettings();
        SettingsChanged();
    }

    public static MorePlanningMod Instance => _instance ?? throw new InvalidOperationException();

    public PlanInfoSet ClipboardPlan { get; set; }


    public bool PlanningVisibility
    {
        get
        {
            EnsureWorldComponentLoaded();
            return _worldComponent?.PlanningVisibility ?? true;
        }
        set
        {
            EnsureWorldComponentLoaded();
            if(_worldComponent == null)
            {
                return;
            }

            if(_worldComponent.PlanningVisibility == value)
            {
                return;
            }

            _worldComponent.PlanningVisibility = value;

            // update the UI icon if present
            MenuUtility.GetPlanningDesignator<VisibilityCommand>()?.UpdateIcon(value);
        }
    }

    public bool OverrideColors => (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ==
        Settings.ShiftKeyForOverride;
}
