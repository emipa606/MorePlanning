using MorePlanning.Common;
using Verse;

namespace MorePlanning.Utility;

public static class MapUtility
{
    public static Designation GetPlanDesignationAt(IntVec3 c, Map map)
    { return map.designationManager.DesignationAt(c, Resources.PlanDesignationDef); }

    public static bool HasAnyPlanDesignationAt(IntVec3 c, Map map)
    { return map.designationManager.DesignationAt(c, Resources.PlanDesignationDef) != null; }

    public static bool HasPlanDesignationAt(IntVec3 c, Map map, int color)
    { return GetPlanDesignationAt(c, map) is PlanDesignation planDesignation && planDesignation.Color == color; }
    public static void RemoveAllPlanDesignationAt(IntVec3 c, Map map)
    { map.designationManager.DesignationAt(c, Resources.PlanDesignationDef)?.Delete(); }
}