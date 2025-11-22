using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Designators;

public class VisibilityCommand : BaseCommand
{
    public VisibilityCommand() : base(
        "MorePlanning.PlanVisibility".Translate(),
        "MorePlanning.PlanVisibilityDesc".Translate())
    { hotKey = KeyBindingDefOf.Misc12; }

    public override void ProcessInput(Event ev)
    {
        CurActivateSound?.PlayOneShotOnCamera();
        MorePlanningMod.Instance.PlanningVisibility = !MorePlanningMod.Instance.PlanningVisibility;
        Find.DesignatorManager.Deselect();
    }

    public override void SelectedUpdate() { UpdateIcon(MorePlanningMod.Instance.PlanningVisibility); }

    public void UpdateIcon(bool visible) { icon = visible ? Resources.IconVisible : Resources.IconInvisible; }
}