using MorePlanning.Plan;
using System.Collections.Generic;
using Verse;

namespace MorePlanning;

public class MorePlanningModSettings : ModSettings
{
    // New: Color strings for the 10 plan colors
    public List<string> PlanColors = new List<string>();
    public int PlanOpacity = 25;
    // Backing fields
    public bool RemoveIfBuildingDespawned = false;
    public bool ShiftKeyForOverride = false;

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref RemoveIfBuildingDespawned, "removeIfBuildingDespawned", false);
        Scribe_Values.Look(ref ShiftKeyForOverride, "shiftKeyForOverride", false);
        Scribe_Values.Look(ref PlanOpacity, "planOpacity", DefaultPlanOpacity);

        Scribe_Collections.Look(ref PlanColors, "planColors", LookMode.Value);

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            if (PlanColors == null || PlanColors.Count != PlanColorManager.NumPlans)
            {
                PlanColors = new List<string>(PlanColorManager.NumPlans);
                for (int i = 0; i < PlanColorManager.NumPlans; i++)
                {
                    PlanColors.Add(PlanColorManager.DefaultColors[i]);
                }
            }

            // do NOT call LoadFromSettings here
            PlanColorManager.InvalidateColors();
        }
    }


    public void ExposeData_ResetToDefaults()
    {
        RemoveIfBuildingDespawned = false;
        ShiftKeyForOverride = false;
        PlanOpacity = DefaultPlanOpacity;

        PlanColors = new List<string>(PlanColorManager.NumPlans);
        for (int i = 0; i < PlanColorManager.NumPlans; i++)
        {
            PlanColors.Add(PlanColorManager.DefaultColors[i]);
        }

        MorePlanningMod.Instance.WriteSettings();
        PlanColorManager.LoadFromSettings();
    }

    public int DefaultPlanOpacity => 25;
}