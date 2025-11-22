using RimWorld;
using UnityEngine;
using Verse;
using Resources = MorePlanning.Common.Resources;

namespace MorePlanning.Designators;

public class PasteDesignator : BaseDesignator
{
    private static float MiddleMouseDownTime;

    public PasteDesignator() : base(
        "MorePlanning.PlanPaste".Translate(),
        "MorePlanning.PlanPasteDesc".Translate(KeyBindingDefOf.Misc1.MainKeyLabel, KeyBindingDefOf.Misc2.MainKeyLabel))
    { icon = Resources.IconPaste; }

    private static void handleRotationShortcuts()
    {
        var rotationDirection = RotationDirection.None;
        if (Event.current.button == 2)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                Event.current.Use();
                MiddleMouseDownTime = Time.realtimeSinceStartup;
            }

            if (Event.current.type == EventType.MouseUp && Time.realtimeSinceStartup - MiddleMouseDownTime < 0.15f)
            {
                rotationDirection = RotationDirection.Clockwise;
            }
        }
        else if (KeyBindingDefOf.Designator_RotateRight.KeyDownEvent)
        {
            rotationDirection = RotationDirection.Clockwise;
        }
        else if (KeyBindingDefOf.Designator_RotateLeft.KeyDownEvent)
        {
            rotationDirection = RotationDirection.Counterclockwise;
        }
        else if (KeyBindingDefOf.Misc1.KeyDownEvent)
        {
            MorePlanningMod.Instance.ClipboardPlan.FlipHorizontally();
        }
        else if (KeyBindingDefOf.Misc2.KeyDownEvent)
        {
            MorePlanningMod.Instance.ClipboardPlan.FlipVertically();
        }

        switch (rotationDirection)
        {
            case RotationDirection.Clockwise:
                MorePlanningMod.Instance.ClipboardPlan.Rotate(RotationDirection.Clockwise);
                break;
            case RotationDirection.Counterclockwise:
                MorePlanningMod.Instance.ClipboardPlan.Rotate(RotationDirection.Counterclockwise);
                break;
        }
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 c) { return true; }

    public override void DesignateSingleCell(IntVec3 c)
    { MorePlanningMod.Instance.ClipboardPlan?.DesignateFromOrigin(c, Map); }

    public override void DoExtraGuiControls(float leftX, float bottomY)
    {
        var winRect = new Rect(leftX, bottomY - 90f, 200f, 90f);
        handleRotationShortcuts();
        Find.WindowStack
            .ImmediateWindow(
                73095,
                winRect,
                WindowLayer.GameUI,
                delegate
                {
                    var rotationDirection = RotationDirection.None;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.Font = GameFont.Medium;
                    var rect = new Rect((winRect.width / 2f) - 64f - 5f, 15f, 64f, 64f);
                    if (Widgets.ButtonImage(rect, TexUI.RotLeftTex))
                    {
                        rotationDirection = RotationDirection.Counterclockwise;
                        Event.current.Use();
                    }

                    Widgets.Label(rect, KeyBindingDefOf.Designator_RotateLeft.MainKeyLabel);
                    var rect2 = new Rect((winRect.width / 2f) + 5f, 15f, 64f, 64f);
                    if (Widgets.ButtonImage(rect2, TexUI.RotRightTex))
                    {
                        rotationDirection = RotationDirection.Clockwise;
                        Event.current.Use();
                    }

                    Widgets.Label(rect2, KeyBindingDefOf.Designator_RotateRight.MainKeyLabel);
                    if (rotationDirection != RotationDirection.None)
                    {
                        MorePlanningMod.Instance.ClipboardPlan.Rotate(rotationDirection);
                    }

                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;
                });
    }

    public override void ProcessInput(Event ev)
    {
        if (MorePlanningMod.Instance.ClipboardPlan == null)
        {
            Messages.Message("MorePlanning.NoCutCopiedPlan".Translate(), MessageTypeDefOf.RejectInput);
        }
        else
        {
            base.ProcessInput(ev);
        }
    }

    public override void Selected()
    {
        base.Selected();
        defaultDesc =
            "MorePlanning.PlanPasteDesc".Translate(
            KeyBindingDefOf.Misc1.MainKeyLabel,
            KeyBindingDefOf.Misc2.MainKeyLabel);
    }

    public override void SelectedUpdate()
    {
        GenDraw.DrawNoBuildEdgeLines();
        if (ArchitectCategoryTab.InfoRect.Contains(UI.MousePositionOnUIInverted) ||
            MorePlanningMod.Instance.ClipboardPlan == null)
        {
            return;
        }

        var intVec = UI.MouseCell();
        MorePlanningMod.Instance.ClipboardPlan.Draw(intVec, Map);
    }

    public override bool DragDrawMeasurements => false;
}