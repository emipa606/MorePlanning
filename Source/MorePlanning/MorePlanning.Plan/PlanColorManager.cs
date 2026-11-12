using System.Collections.Generic;
using MorePlanning.Utility;
using Multiplayer.API;
using UnityEngine;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Plan;

public static class PlanColorManager
{
    public const int NumPlans = 10;

    private static readonly int ColorShaderId = Shader.PropertyToID("_Color");

    private static readonly Color[] PlanColor = new Color[NumPlans];
    private static readonly bool[] PlanColorChanged = new bool[NumPlans];

    public static readonly string[] DefaultColors =
        ["a9a9a9", "2095f2", "4bae4f", "f34235", "feea3a", "ff00f0", "00fffc", "8400ff", "ffa200", "000000"];

    private static MorePlanningModSettings Settings => MorePlanningMod.Instance.Settings;


    /* --------------------------------------------------------------
     *  INTERNAL COLOR MATERIAL UPDATE
     * -------------------------------------------------------------- */

    private static void OnColorChanged(int numColor)
    {
        var hex = Settings.PlanColors[numColor];
        PlanColor[numColor] = hex.HexToColor();
        PlanColorChanged[numColor] = true;
    }


    /* --------------------------------------------------------------
     *  SYNCED COLOR CHANGE (Multiplayer)
     * -------------------------------------------------------------- */

    [SyncMethod]
    public static void ChangeColor(int colorNum, string hexColor)
    {
        Settings.PlanColors[colorNum] = hexColor;

        OnColorChanged(colorNum);

        // Ensure RimWorld marks settings as modified
        MorePlanningMod.Instance.WriteSettings();
    }


    /* --------------------------------------------------------------
     *  PUBLIC GETTERS
     * -------------------------------------------------------------- */

    public static Color GetColor(int col = -1)
    {
        if (col < 0)
        {
            col = MorePlanningMod.Instance.SelectedColor;
        }

        return PlanColor[col];
    }

    public static Material GetMaterial(int colorIndex)
    {
        if (!PlanColorChanged[colorIndex])
        {
            return Resources.PlanMatColor[colorIndex];
        }

        PlanColorChanged[colorIndex] = false;

        var c = PlanColor[colorIndex];
        c.a = Settings.PlanOpacity / 100f;

        Resources.PlanMatColor[colorIndex].SetColor(ColorShaderId, c);
        return Resources.PlanMatColor[colorIndex];
    }

    public static void InvalidateColors()
    {
        for (var i = 0; i < NumPlans; i++)
        {
            PlanColorChanged[i] = true;
        }
    }


    /* --------------------------------------------------------------
     *  INITIALIZATION
     * -------------------------------------------------------------- */

    /// <summary>
    ///     Called after settings load (PostLoadInit) or after reset. Ensures colors are valid and pushes them into
    ///     materials.
    /// </summary>
    public static void LoadFromSettings()
    {
        // Ensure list exists and contains correct number of entries
        if (Settings.PlanColors is not { Count: NumPlans })
        {
            Settings.PlanColors = new List<string>(NumPlans);
            for (var i = 0; i < NumPlans; i++)
            {
                Settings.PlanColors.Add(DefaultColors[i]);
            }
        }

        // Initialize all cached colors/materials
        for (var i = 0; i < NumPlans; i++)
        {
            OnColorChanged(i);
        }
    }
}