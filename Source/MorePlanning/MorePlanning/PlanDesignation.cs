using MorePlanning.Plan;
using UnityEngine;
using Verse;

namespace MorePlanning;

public class PlanDesignation : Designation
{
    public int Color;

    public PlanDesignation()
    {
    }

    public PlanDesignation(LocalTargetInfo target, DesignationDef def, int color) : base(target, def) { Color = color; }

    public override void DesignationDraw()
    {
        if(!MorePlanningMod.Instance.PlanningVisibility)
        {
            return;
        }

        var position = target.Cell.ToVector3ShiftedWithAltitude(DesignationDrawAltitude);
        Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, PlanColorManager.GetMaterial(Color), 0);
    }
}
