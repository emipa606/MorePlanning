using MorePlanning.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Plan;

public class PlanInfoSet : IEnumerable<PlanInfo>
{
    private readonly List<PlanInfo> PlanDesignationInfo;

    public PlanInfoSet(List<PlanInfo> planDesignationInfo)
    {
        PlanDesignationInfo = planDesignationInfo;
        var num = planDesignationInfo.Min(plan => plan.Pos.x);
        var num2 = planDesignationInfo.Max(plan => plan.Pos.z);
        var num3 = planDesignationInfo.Max(plan => plan.Pos.x);
        var num4 = planDesignationInfo.Min(plan => plan.Pos.z);
        Size = new IntVec2(num3 - num + 1, num2 - num4 + 1);
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    internal void FlipHorizontally()
    {
        var num = PlanDesignationInfo.Min(plan => plan.Pos.x);
        var num2 = (PlanDesignationInfo.Max(plan => plan.Pos.x) + num) / 2f;
        foreach (var item in PlanDesignationInfo)
        {
            if (item.Pos.x < num2)
            {
                item.Pos.x = (int)(num2 + (num2 - item.Pos.x));
            }
            else if (item.Pos.x > num2)
            {
                item.Pos.x = (int)(num2 - (item.Pos.x - num2));
            }
        }
    }

    internal void FlipVertically()
    {
        var num = PlanDesignationInfo.Min(plan => plan.Pos.z);
        var num2 = (PlanDesignationInfo.Max(plan => plan.Pos.z) + num) / 2f;
        foreach (var item in PlanDesignationInfo)
        {
            if (item.Pos.z < num2)
            {
                item.Pos.z = (int)(num2 + (num2 - item.Pos.z));
            }
            else if (item.Pos.z > num2)
            {
                item.Pos.z = (int)(num2 - (item.Pos.z - num2));
            }
        }
    }

    public void DesignateFromOrigin(IntVec3 c, Map map)
    {
        var planDesignationDef = Resources.PlanDesignationDef;
        foreach (var item in PlanDesignationInfo)
        {
            var intVec = item.Pos + c;
            if (intVec.InNoBuildEdgeArea(map))
            {
                continue;
            }

            MapUtility.RemoveAllPlanDesignationAt(intVec, map);
            map.designationManager.AddDesignation(new PlanDesignation(intVec, planDesignationDef, item.Color));
        }
    }

    public void Draw(IntVec3 intVec, Map map)
    {
        var list = new List<IntVec3>();
        foreach (var item in PlanDesignationInfo)
        {
            var intVec2 = item.Pos + intVec;
            if (intVec2.InNoBuildEdgeArea(map))
            {
                continue;
            }

            var position = intVec2.ToVector3ShiftedWithAltitude(AltitudeLayer.Silhouettes.AltitudeFor());
            Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, Resources.PlanMatColor[item.Color], 0);
            list.Add(intVec2);
        }

        GenDraw.DrawFieldEdges(list);
    }

    public IEnumerator<PlanInfo> GetEnumerator() { return PlanDesignationInfo.GetEnumerator(); }

    public void Rotate(RotationDirection rotationDirection)
    {
        foreach (var item in PlanDesignationInfo)
        {
            item.Pos = item.Pos.RotatedBy(rotationDirection == RotationDirection.Clockwise ? Rot4.East : Rot4.West);
        }
    }

    public IntVec2 Size { get; private set; }
}