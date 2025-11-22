using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MorePlanning.Designators;

public abstract class BaseDesignator : Designator
{
    protected BaseDesignator(string label, string desc)
    {
        defaultLabel = label;
        defaultDesc = desc;
        soundSucceeded = SoundDefOf.Designate_PlanAdd;
        soundDragSustain = SoundDefOf.Designate_DragStandard;
        soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
        useMouseIcon = true;
    }

    protected virtual void DrawToolbarIcon(Rect rect)
    {
        var material = !disabled ? null : TexUI.GrayscaleGUI;
        var badTex = icon;
        if(badTex == null)
        {
            badTex = BaseContent.BadTex;
        }

        var rect2 = rect;
        var position = rect2.position;
        var x = iconOffset.x * rect2.size.x;
        var y = iconOffset.y;
        rect2.position = position + new Vector2(x, y * rect2.size.y);
        Widgets.DrawTextureFitted(
            rect2,
            badTex,
            iconDrawScale * 0.85f,
            iconProportions,
            iconTexCoords,
            iconAngle,
            material);
    }

    protected override GizmoResult GizmoOnGUIInt(Rect rect, GizmoRenderParms parms)
    {
        Text.Font = GameFont.Tiny;
        var mouseIsOver = false;
        if(Mouse.IsOver(rect))
        {
            mouseIsOver = true;
            if(!disabled)
            {
                GUI.color = GenUI.MouseoverColor;
            }
        }

        var material = !disabled ? null : TexUI.GrayscaleGUI;
        GenUI.DrawTextureWithMaterial(rect, BGTex, material);
        MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
        GUI.color = IconDrawColor;
        DrawToolbarIcon(rect);
        GUI.color = Color.white;
        var keyIsDown = false;
        var keyCode = hotKey?.MainKey ?? KeyCode.None;
        if(keyCode != KeyCode.None && !GizmoGridDrawer.drawnHotKeys.Contains(keyCode))
        {
            Widgets.Label(new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, 18f), keyCode.ToStringReadable());
            GizmoGridDrawer.drawnHotKeys.Add(keyCode);
            if(hotKey is { KeyDownEvent: true })
            {
                keyIsDown = true;
                Event.current.Use();
            }
        }

        if(Widgets.ButtonInvisible(rect))
        {
            keyIsDown = true;
        }

        var labelCap = LabelCap;
        if(!labelCap.NullOrEmpty())
        {
            var num = Text.CalcHeight(labelCap, rect.width);
            var rect2 = new Rect(rect.x, rect.yMax - num + 12f, rect.width, num);
            GUI.DrawTexture(rect2, TexUI.GrayTextBG);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect2, labelCap);
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        GUI.color = Color.white;
        if(DoTooltip)
        {
            TipSignal tip = Desc;
            if(disabled && !disabledReason.NullOrEmpty())
            {
                var text = tip.text;
                tip.text = $"{text}\n\n" + "DisabledCommand".Translate() + ": " + disabledReason;
            }

            TooltipHandler.TipRegion(rect, tip);
        }

        if(!HighlightTag.NullOrEmpty() &&
            (Find.WindowStack.FloatMenu == null || !Find.WindowStack.FloatMenu.windowRect.Overlaps(rect)))
        {
            UIHighlighter.HighlightOpportunity(rect, HighlightTag);
        }

        Text.Font = GameFont.Small;
        if(!keyIsDown)
        {
            return mouseIsOver ? new GizmoResult(GizmoState.Mouseover, null) : new GizmoResult(GizmoState.Clear, null);
        }

        if(disabled)
        {
            if(!disabledReason.NullOrEmpty())
            {
                Messages.Message(disabledReason, MessageTypeDefOf.RejectInput, false);
            }

            return new GizmoResult(GizmoState.Mouseover, null);
        }

        GizmoResult result;
        if(Event.current.button == 1)
        {
            result = new GizmoResult(GizmoState.OpenedFloatMenu, Event.current);
        } else
        {
            if(!TutorSystem.AllowAction(TutorTagSelect))
            {
                return new GizmoResult(GizmoState.Mouseover, null);
            }

            result = new GizmoResult(GizmoState.Interacted, Event.current);
            TutorSystem.Notify_Event(TutorTagSelect);
        }

        return result;
    }
}