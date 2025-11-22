using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MorePlanning.Utility;

internal class MenuUtility
{
    private static FieldInfo _resolvedDesignatorsInfo;

    private static List<Designator> GetPlanningDesignators()
    {
        if (_resolvedDesignatorsInfo == null)
        {
            InitReflection();
        }

        var named = DefDatabase<DesignationCategoryDef>.GetNamed("Planning");
        if (named != null)
        {
            return (List<Designator>)_resolvedDesignatorsInfo?.GetValue(named);
        }

        MorePlanningMod.LogError("Menu planning not found");
        return null;
    }

    private static void InitReflection()
    {
        _resolvedDesignatorsInfo =
            typeof(DesignationCategoryDef).GetField(
            "resolvedDesignators",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (_resolvedDesignatorsInfo == null)
        {
            MorePlanningMod.LogError(
                "Reflection failed (MenuUtility::InitReflection, DesignationCategoryDef.resolvedDesignators)");
        }
    }

    public static T GetPlanningDesignator<T>() where T : class
    {
        foreach (var planningDesignator in GetPlanningDesignators())
        {
            if (planningDesignator is T result)
            {
                return result;
            }
        }

        return null;
    }
}