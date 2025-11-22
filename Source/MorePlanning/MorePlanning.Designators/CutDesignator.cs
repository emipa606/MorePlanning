using MorePlanning.Common;
using MorePlanning.Utility;
using System.Collections.Generic;
using Verse;

namespace MorePlanning.Designators;

public class CutDesignator : CopyDesignator
{
    public CutDesignator()
    {
        defaultLabel = "MorePlanning.PlanCut".Translate();
        defaultDesc = "MorePlanning.PlanCutDesc".Translate();
        icon = Resources.IconCut;
    }

    public override void DesignateMultiCell(IEnumerable<IntVec3> cells)
    {
        base.DesignateMultiCell(cells);
        foreach(var cell in cells)
        {
            MapUtility.RemoveAllPlanDesignationAt(cell, Map);
        }
    }

    public override void RenderHighlight(List<IntVec3> dragCells)
    { DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells); }
}