using System.Collections.Generic;
using MorePlanning.Plan;
using Verse;

namespace MorePlanning;

public class MorePlanningModSettings : ModSettings
{
    // New: Color strings for the 10 plan colors
    public List<string> PlanColors = [];

    public int PlanOpacity = 25;

    // Backing fields
    public bool RemoveIfBuildingDespawned;
    public bool ShiftKeyForOverride;

    public static int DefaultPlanOpacity => 25;

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref RemoveIfBuildingDespawned, "removeIfBuildingDespawned");
        Scribe_Values.Look(ref ShiftKeyForOverride, "shiftKeyForOverride");
        Scribe_Values.Look(ref PlanOpacity, "planOpacity", DefaultPlanOpacity);

        Scribe_Collections.Look(ref PlanColors, "planColors", LookMode.Value);

        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        if (PlanColors is not { Count: PlanColorManager.NumPlans })
        {
            PlanColors = new List<string>(PlanColorManager.NumPlans);
            for (var i = 0; i < PlanColorManager.NumPlans; i++)
            {
                PlanColors.Add(PlanColorManager.DefaultColors[i]);
            }
        }

        // do NOT call LoadFromSettings here
        PlanColorManager.InvalidateColors();
    }


    public void ExposeData_ResetToDefaults()
    {
        RemoveIfBuildingDespawned = false;
        ShiftKeyForOverride = false;
        PlanOpacity = DefaultPlanOpacity;

        PlanColors = new List<string>(PlanColorManager.NumPlans);
        for (var i = 0; i < PlanColorManager.NumPlans; i++)
        {
            PlanColors.Add(PlanColorManager.DefaultColors[i]);
        }

        MorePlanningMod.Instance.WriteSettings();
        PlanColorManager.LoadFromSettings();
    }
}