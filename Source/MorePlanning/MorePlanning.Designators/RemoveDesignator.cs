using MorePlanning.Plan;
using MorePlanning.Utility;
using RimWorld;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Designators;

public class RemoveDesignator : PlanBaseDesignator
{
    public RemoveDesignator() : base("DesignatorPlanRemove".Translate(), "DesignatorPlanRemoveDesc".Translate())
    {
        soundSucceeded = SoundDefOf.Designate_PlanRemove;
        hotKey = KeyBindingDefOf.Designator_Deconstruct;
    }

    protected override void DrawToolbarIcon(Rect rect)
    {
        var screenRect = new Rect(0f, 0f, iconProportions.x, iconProportions.y);
        var num = !(screenRect.width / screenRect.height < rect.width / rect.height)
            ? rect.width / screenRect.width
            : rect.height / screenRect.height;
        num *= 0.85f;
        screenRect.width *= num;
        screenRect.height *= num;
        screenRect.x = rect.x + (rect.width / 2f) - (screenRect.width / 2f);
        screenRect.y = rect.y + (rect.height / 2f) - (screenRect.height / 2f);
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        Graphics.DrawTexture(screenRect, Resources.Plan, iconTexCoords, 0, 1, 0, 1, PlanColorManager.GetColor());
        Widgets.DrawTextureFitted(
            new Rect(rect),
            Resources.RemoveIcon,
            iconDrawScale * 0.85f,
            iconProportions,
            iconTexCoords);
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
        if (!c.InBounds(Map))
        {
            return false;
        }

        return MorePlanningMod.Instance.OverrideColors
            ? MapUtility.HasAnyPlanDesignationAt(c, Map)
            : MapUtility.HasPlanDesignationAt(c, Map, MorePlanningMod.Instance.SelectedColor);
    }

    public override void DesignateSingleCell(IntVec3 c) { MapUtility.RemoveAllPlanDesignationAt(c, Map); }

    public override void DrawMouseAttachments()
    {
        var mousePosition = Event.current.mousePosition;
        var y = mousePosition.y + 12f;
        if (MorePlanningMod.Instance.OverrideColors)
        {
            Widgets.DrawTextureFitted(
                new Rect(mousePosition.x + 12f, y, 32f, 32f),
                Resources.PlanToolRemoveAll,
                iconDrawScale);
            return;
        }

        Graphics.DrawTexture(
            new Rect(mousePosition.x + 12f, y, 32f, 32f),
            Resources.Plan,
            iconTexCoords,
            0,
            1,
            0,
            1,
            PlanColorManager.GetColor());
        Widgets.DrawTextureFitted(new Rect(mousePosition.x + 12f, y, 32f, 32f), Resources.RemoveIcon, iconDrawScale);
    }
}