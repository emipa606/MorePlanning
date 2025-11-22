using RimWorld;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Designators;

public class ExportCommand : BaseCommand
{
    public ExportCommand() : base("MorePlanning.PlanExport".Translate(), "MorePlanning.PlanExportDesc".Translate())
    { icon = Resources.IconExport; }

    protected override void OnClick()
    {
        var clipboardPlan = MorePlanningMod.Instance.ClipboardPlan;
        if (clipboardPlan == null)
        {
            Messages.Message("MorePlanning.PlanExportError".Translate(), MessageTypeDefOf.RejectInput);
            return;
        }

        var stringBuilder = new StringBuilder();
        var num = clipboardPlan.Min(x => x.Pos.x);
        var num2 = clipboardPlan.Max(x => x.Pos.x);
        var num3 = clipboardPlan.Min(x => x.Pos.z);
        var num4 = clipboardPlan.Max(x => x.Pos.z);
        var num5 = num2 - num + 1;
        var num6 = num4 - num3 + 1;
        var array = new int[num5, num6];
        for (var num7 = 0; num7 < num5; num7++)
        {
            for (var num8 = 0; num8 < num6; num8++)
            {
                array[num7, num8] = -1;
            }
        }

        foreach (var item in clipboardPlan)
        {
            var num9 = item.Pos.x - num;
            var num10 = item.Pos.z - num3;
            array[num9, num10] = item.Color;
        }

        for (var num11 = num6 - 1; num11 >= 0; num11--)
        {
            for (var num12 = 0; num12 < num5; num12++)
            {
                if (array[num12, num11] == -1)
                {
                    stringBuilder.Append('-');
                }
                else
                {
                    stringBuilder.Append((char)(97 + array[num12, num11]));
                }
            }

            stringBuilder.AppendLine();
        }

        GUIUtility.systemCopyBuffer = stringBuilder.ToString();
        Messages.Message("MorePlanning.PlanExportOK".Translate(), MessageTypeDefOf.NeutralEvent);
    }
}