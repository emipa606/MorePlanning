using MorePlanning.Plan;
using MorePlanning.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Designators;

public class ImportCommand : BaseCommand
{
    public ImportCommand() : base("MorePlanning.PlanImport".Translate(), "MorePlanning.PlanImportDesc".Translate())
    { icon = Resources.IconImport; }

    protected override void OnClick()
    {
        try
        {
            var systemCopyBuffer = GUIUtility.systemCopyBuffer;
            if (string.IsNullOrWhiteSpace(systemCopyBuffer))
            {
                Messages.Message("MorePlanning.PlanImportError".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            var array = systemCopyBuffer.ToLowerInvariant().Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(array);
            var regex = new Regex("^[-a-z]+$");
            if (!array.All(regex.IsMatch))
            {
                Messages.Message("MorePlanning.PlanImportError".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            _ = array.Length;
            var w = array[0].Length;
            if (array.Any(x => x.Length != w))
            {
                Messages.Message("MorePlanning.PlanImportError".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            var list = new List<PlanInfo>();
            for (var num = 0; num < array.Length; num++)
            {
                var text = array[num];
                for (var num2 = 0; num2 < text.Length; num2++)
                {
                    var c = text[num2];
                    if (c == '-')
                    {
                        continue;
                    }

                    var color = Mathf.Min(10, Mathf.Max(0, c - 97));
                    list.Add(new PlanInfo { Pos = new IntVec3(num2, 0, num), Color = color });
                }
            }

            MorePlanningMod.Instance.ClipboardPlan = new PlanInfoSet(list);
            var planningDesignator = MenuUtility.GetPlanningDesignator<PasteDesignator>();
            Find.DesignatorManager.Select(planningDesignator);
            Messages.Message("MorePlanning.PlanImportOK".Translate(), MessageTypeDefOf.RejectInput);
        }
        catch
        {
            Messages.Message("MorePlanning.PlanImportError".Translate(), MessageTypeDefOf.RejectInput);
        }
    }
}