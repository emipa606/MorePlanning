using UnityEngine;
using Verse;

namespace MorePlanning.Common;

[StaticConstructorOnStartup]
internal static class Resources
{
    public static readonly Texture2D ColorPickerOverlay;

    public static readonly Texture2D ColorPickerSelect;

    public static readonly Texture2D HsvSlider;

    public static readonly Texture2D IconCopy;

    public static readonly Texture2D IconCut;

    public static readonly Texture2D IconExport;

    public static readonly Texture2D IconImport;

    public static readonly Texture2D IconInvisible;

    public static readonly Texture2D IconOpacity;

    public static readonly Texture2D IconPaste;

    public static readonly Texture2D IconShowPlanning;

    public static readonly Texture2D IconVisible;
    public static readonly Texture2D Plan;

    public static DesignationDef PlanDesignationDef;

    public static readonly Material[] PlanMatColor;

    public static readonly Texture2D PlanToolRemoveAll;

    public static readonly Texture2D RemoveIcon;

    public static readonly Texture2D ToolBoxColor;

    public static readonly Texture2D ToolBoxColorSelected;

    static Resources()
    {
        Plan = ContentFinder<Texture2D>.Get("UI/Plan");
        IconOpacity = ContentFinder<Texture2D>.Get("UI/Opacity");
        IconShowPlanning = ContentFinder<Texture2D>.Get("UI/ShowPlanning");
        IconVisible = ContentFinder<Texture2D>.Get("UI/PlanVisible");
        IconInvisible = ContentFinder<Texture2D>.Get("UI/PlanInvisible");
        ToolBoxColor = ContentFinder<Texture2D>.Get("UI/ToolBoxColor");
        ToolBoxColorSelected = ContentFinder<Texture2D>.Get("UI/ToolBoxColorSelected");
        RemoveIcon = ContentFinder<Texture2D>.Get("UI/RemoveIcon");
        PlanToolRemoveAll = ContentFinder<Texture2D>.Get("UI/PlanToolRemoveAll");
        ColorPickerSelect = ContentFinder<Texture2D>.Get("UI/ColorPickerSelect");
        ColorPickerOverlay = ContentFinder<Texture2D>.Get("UI/ColorPickerOverlay");
        HsvSlider = ContentFinder<Texture2D>.Get("UI/HsvSlider");
        IconExport = ContentFinder<Texture2D>.Get("UI/PlanExport");
        IconImport = ContentFinder<Texture2D>.Get("UI/PlanImport");
        IconCopy = ContentFinder<Texture2D>.Get("UI/PlanCopy");
        IconCut = ContentFinder<Texture2D>.Get("UI/PlanCut");
        IconPaste = ContentFinder<Texture2D>.Get("UI/PlanPaste");
        PlanMatColor = new Material[10];
        for(var i = 0; i < PlanMatColor.Length; i++)
        {
            PlanMatColor[i] = MaterialPool.MatFrom($"UI/PlanBase{i}", ShaderDatabase.MetaOverlay, Color.white);
        }
    }
}