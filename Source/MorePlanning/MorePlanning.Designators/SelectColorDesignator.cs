using MorePlanning.Dialogs;
using MorePlanning.Plan;
using MorePlanning.Utility;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Designators;

public class SelectColorDesignator(int color) : BaseDesignator(color.ToString(), "MorePlanning.PlanDesc".Translate())
{
    protected override void DrawToolbarIcon(Rect rect)
    {
        var rect2 = rect;
        var position = rect2.position;
        var x = iconOffset.x * rect2.size.x;
        var y = iconOffset.y;
        rect2.position = position + new Vector2(x, y * rect2.size.y);
        var rect3 = new Rect(0f, 0f, iconProportions.x, iconProportions.y);
        var num = !(rect3.width / rect3.height < rect.width / rect.height)
            ? rect.width / rect3.width
            : rect.height / rect3.height;
        num *= iconDrawScale * 0.85f;
        rect3.width *= num;
        rect3.height *= num;
        rect3.x = rect.x + (rect.width / 2f) - (rect3.width / 2f);
        rect3.y = rect.y + (rect.height / 2f) - (rect3.height / 2f);
        Widgets.DrawBoxSolid(rect3, PlanColorManager.GetColor(color));
        Widgets.DrawTextureFitted(
            rect2,
            MorePlanningMod.Instance.SelectedColor == color ? Resources.ToolBoxColorSelected : Resources.ToolBoxColor,
            iconDrawScale * 0.85f,
            iconProportions,
            iconTexCoords);
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 loc) { return AcceptanceReport.WasRejected; }

    public override void ProcessInput(Event ev)
    {
        var options = new List<FloatMenuOption>
        {
            new(
            "MorePlanning.ChangeColor".Translate(),
            delegate
            {
                Find.WindowStack.Add(new ColorSelectorDialog(color));
            })
        };
        Find.WindowStack.Add(new FloatMenu(options));
        MorePlanningMod.Instance.SelectedColor = color;
        var planningDesignator = MenuUtility.GetPlanningDesignator<AddDesignator>();
        Find.DesignatorManager.Select(planningDesignator);
    }

    public override string LabelCap { get; } = string.Empty;
}