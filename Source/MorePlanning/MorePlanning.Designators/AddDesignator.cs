using MorePlanning.Plan;
using MorePlanning.Utility;
using RimWorld;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Designators;

public class AddDesignator : PlanBaseDesignator
{
    public AddDesignator()
        : base("DesignatorPlan".Translate(), "MorePlanning.PlanDesc".Translate())
    {
        hotKey = KeyBindingDefOf.Designator_Cancel;
    }

    protected override void DrawToolbarIcon(Rect rect)
    {
        Graphics.DrawTexture(new Rect(rect), Resources.Plan, iconTexCoords, 0, 1, 0, 1, PlanColorManager.GetColor());
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        if (!loc.InBounds(Map))
        {
            return false;
        }

        if (loc.InNoBuildEdgeArea(Map))
        {
            return "TooCloseToMapEdge".Translate();
        }

        return MorePlanningMod.Instance.OverrideColors || !MapUtility.HasAnyPlanDesignationAt(loc, Map);
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        MapUtility.RemoveAllPlanDesignationAt(c, Map);
        var newDes = new PlanDesignation(c, Resources.PlanDesignationDef, MorePlanningMod.Instance.SelectedColor);
        Map.designationManager.AddDesignation(newDes);
    }

    public override void DrawMouseAttachments()
    {
        var mousePosition = Event.current.mousePosition;
        Graphics.DrawTexture(new Rect(y: mousePosition.y + 12f, x: mousePosition.x + 12f, width: 32f, height: 32f),
            Resources.Plan, iconTexCoords, 0, 1, 0, 1, PlanColorManager.GetColor());
    }
}