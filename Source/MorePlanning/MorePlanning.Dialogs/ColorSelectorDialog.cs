using MorePlanning.Common;
using MorePlanning.Plan;
using System;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Dialogs;

public class ColorSelectorDialog : Window
{
    private bool AcceptColor;
    private readonly ConvertibleColor Color;

    private readonly string HexColorBefore;

    private string InputColorHex;

    private readonly int NumColor;

    private float S;

    private float Slider;

    private float V;

    public ColorSelectorDialog(int numColor)
    {
        NumColor = numColor;
        Color = new ConvertibleColor(PlanColorManager.GetColor(numColor));
        InputColorHex = Color.ColorHex;
        Slider = Color.H;
        S = Color.S;
        V = Color.V;
        HexColorBefore = InputColorHex;
        forcePause = true;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnClickedOutside = true;
    }

    public override void DoWindowContents(Rect inRect)
    {
        var colorRGB = Color.ColorRGB;
        var rect = new Rect(0f, 0f, 10f, 10f) { center = new Vector2(256f * S, 256f - (256f * V)) };
        if(GUI.RepeatButton(new Rect(0f, 0f, 256f, 256f), string.Empty))
        {
            Color.S = (Event.current.mousePosition.x - inRect.x) / 256f;
            Color.V = 1f - ((Event.current.mousePosition.y - inRect.y) / 256f);
        }

        Widgets.DrawBoxSolid(new Rect(0f, 0f, 256f, 256f), Color.HueOnly);
        GUI.DrawTexture(new Rect(0f, 0f, 256f, 256f), Resources.ColorPickerOverlay);
        GUI.DrawTexture(rect, Resources.ColorPickerSelect);
        GUI.DrawTexture(new Rect(275f, 0f, 19f, 256f), Resources.HsvSlider);
        var num = GUI.VerticalSlider(new Rect(264f, 0f, 11f, 256f), Slider, 1f, 0f);
        Widgets.DrawBoxSolid(new Rect(305f, 0f, 76f, 76f), Color.ColorRGB);
        var text = Widgets.TextField(new Rect(305f, 91f, 76f, 23f), InputColorHex);
        var num2 = Widgets.ButtonText(new Rect(305f, 128f, 76f, 50f), "MorePlanning.DefaultColor".Translate());
        var okPressed = Widgets.ButtonText(new Rect(305f, 234f, 76f, 23f), "MorePlanning.Ok".Translate());
        if(Math.Abs(num - Slider) > 0.01)
        {
            Color.H = num;
            Slider = num;
        }

        var colorChanged = false;
        if(text != InputColorHex)
        {
            Color.ColorHex = $"#{text}";
            InputColorHex = text;
            colorChanged = true;
        }

        if(num2)
        {
            Color.ColorHex = $"#{PlanColorManager.DefaultColors[NumColor]}";
        }

        if(okPressed)
        {
            AcceptColor = true;
            Close();
        }

        if(colorRGB.Equals(Color.ColorRGB))
        {
            return;
        }

        Slider = Color.H;
        S = Color.S;
        V = Color.V;
        if(!colorChanged)
        {
            InputColorHex = Color.ColorHex;
        }

        PlanColorManager.ChangeColor(NumColor, Color.ColorHex);
    }

    public override void PreClose()
    {
        if(!AcceptColor)
        {
            PlanColorManager.ChangeColor(NumColor, HexColorBefore);
        }
    }

    public override Vector2 InitialSize => new(416f, 292f);
}