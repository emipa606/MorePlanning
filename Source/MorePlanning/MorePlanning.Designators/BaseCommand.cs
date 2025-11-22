using UnityEngine;
using Verse;

namespace MorePlanning.Designators;

public class BaseCommand : Designator
{
    protected BaseCommand(string label, string desc)
    {
        defaultLabel = label;
        defaultDesc = desc;
    }

    protected virtual void OnClick()
    {
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        return AcceptanceReport.WasRejected;
    }

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        OnClick();
    }
}