using System;
using System.Collections.Generic;
using System.Linq;
using MorePlanning.Common;
using MorePlanning.Plan;
using MorePlanning.Utility;
using RimWorld;
using Verse;

namespace MorePlanning.Designators;

public class CopyDesignator : BaseDesignator
{
    // ReSharper disable once MemberCanBeProtected.Global
    public CopyDesignator() : base("MorePlanning.PlanCopy".Translate(), "MorePlanning.PlanCopyDesc".Translate())
    { icon = Resources.IconCopy; }

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
        if (!c.InBounds(Map))
        {
            return false;
        }

        if (c.InNoBuildEdgeArea(Map))
        {
            return "TooCloseToMapEdge".Translate();
        }

        return MapUtility.HasAnyPlanDesignationAt(c, Map);
    }

    public override void DesignateMultiCell(IEnumerable<IntVec3> cells)
    {
        var list = (from cell in cells
                    select MapUtility.GetPlanDesignationAt(cell, Map)
            into designation
                    where designation != null
                    select designation).ToList();
        cells = list.Select(plan => plan.target.Cell);
        if (list.Count == 0)
        {
            Messages.Message("MorePlanning.MissingPlanningDesignations".Translate(), MessageTypeDefOf.RejectInput);
            return;
        }

        var num = cells.Min(cell => cell.x);
        var num2 = cells.Max(cell => cell.z);
        var x = cells.Max(cell => cell.x);
        var num3 = cells.Min(cell => cell.z);
        var intVec = new IntVec2((int)Math.Floor(UI.MouseMapPosition().x), (int)Math.Floor(UI.MouseMapPosition().z));
        if (intVec.x < num)
        {
            intVec.x = num;
        }
        else if (intVec.x > num)
        {
            intVec.x = x;
        }

        if (intVec.z > num2)
        {
            intVec.z = num2;
        }
        else if (intVec.z < num3)
        {
            intVec.z = num3;
        }

        var x2 = intVec.x;
        var z = intVec.z;
        var list2 = new List<PlanInfo>();
        foreach (var item2 in list)
        {
            var planDesignation = item2 as PlanDesignation;
            var item = new PlanInfo
            {
                Color = planDesignation?.Color ?? 0,
                Pos = new IntVec3(item2.target.Cell.x - x2, item2.target.Cell.y, item2.target.Cell.z - z)
            };
            list2.Add(item);
        }

        var clipboardPlan = new PlanInfoSet(list2);
        MorePlanningMod.Instance.ClipboardPlan = clipboardPlan;
        Finalize(true);
        var planningDesignator = MenuUtility.GetPlanningDesignator<PasteDesignator>();
        Find.DesignatorManager.Select(planningDesignator);
    }

    public override void RenderHighlight(List<IntVec3> dragCells)
    { DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells); }

    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Plans;

    public override bool DragDrawMeasurements => true;
}