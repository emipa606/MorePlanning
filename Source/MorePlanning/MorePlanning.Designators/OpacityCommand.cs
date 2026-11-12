using System.Collections.Generic;
using MorePlanning.Common;
using Verse;

namespace MorePlanning.Designators;

public class OpacityCommand : BaseCommand
{
    private static readonly int[] _opacityOptions = [10, 15, 20, 25, 30, 40, 50, 60, 70, 80];

    public OpacityCommand() : base("MorePlanning.Opacity.label".Translate(0), "MorePlanning.Opacity.desc".Translate())
    {
        icon = Resources.IconOpacity;
    }

    private void UpdateLabel(int value)
    {
        defaultLabel = "MorePlanning.Opacity.label".Translate(value);
    }

    protected override void OnClick()
    {
        var list = new List<FloatMenuOption>();
        var opacityOptions = _opacityOptions;
        foreach (var num in opacityOptions)
        {
            var text = $"{num}%";
            if (num == MorePlanningModSettings.DefaultPlanOpacity)
            {
                text += " " + "MorePlanning.DefaultOpacity".Translate();
            }

            var value1 = num;
            list.Add(
                new FloatMenuOption(
                    text,
                    delegate
                    {
                        MorePlanningMod.Instance.Settings.PlanOpacity = value1;
                        MorePlanningMod.Instance.WriteSettings();
                        UpdateLabel(value1);
                    }));
        }

        Find.WindowStack.Add(new FloatMenu(list));
        Find.DesignatorManager.Deselect();
    }

    public override void SelectedUpdate()
    {
        UpdateLabel(MorePlanningModSettings.DefaultPlanOpacity);
    }
}