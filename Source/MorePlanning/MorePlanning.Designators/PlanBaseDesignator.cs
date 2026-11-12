using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MorePlanning.Designators;

public abstract class PlanBaseDesignator(string label, string desc) : BaseDesignator(label, desc)
{
    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Plans;

    public override bool DragDrawMeasurements => true;

    public override void RenderHighlight(List<IntVec3> dragCells)
    {
        DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);
    }

    public override void SelectedUpdate()
    {
        GenUI.RenderMouseoverBracket();
        GenDraw.DrawNoBuildEdgeLines();
        MorePlanningMod.Instance.PlanningVisibility = true;
    }
}