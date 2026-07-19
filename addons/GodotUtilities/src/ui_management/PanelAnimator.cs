using System.Collections.Generic;
using Godot;

namespace GodotUtilities.UI;

public enum PanelTransition
{
    Constant,
    Fade,
    Pop,
    SlideV,
    SlideH,
}

public static class PanelAnimator
{
    private const float FADE_DURATION  = 0.1f;
    private const float POP_DURATION   = 0.15f;
    
    private const float SLIDE_DURATION = 0.2f;
    private const float SLIDE_AMOUNT   = 10f;

    private static readonly Dictionary<UIPanel, Vector2> defaultPositions = new();

    public static void ClearPosition(UIPanel panel) => defaultPositions.Remove(panel);

    public static Tween Animate(UIPanel panel, PanelTransition transition, bool isOpen)
    {
        Tween tween = panel.CreateTween();

        switch (transition)
        {
            case PanelTransition.Pop: OnPop(panel, tween, isOpen); break;
            case PanelTransition.Fade: OnFade(panel, tween, isOpen); break;
            case PanelTransition.SlideV: OnSlide(panel, tween, isOpen, Vector2.Down); break;
            case PanelTransition.SlideH: OnSlide(panel, tween, isOpen, Vector2.Right); break;
            case PanelTransition.Constant: tween.TweenCallback(Callable.From(DummyMethod)); break;
        }

        return tween;
    }

    private static void OnFade(UIPanel panel, Tween tween, bool isOpen)
    {
        tween.TweenFade(panel, isOpen.ToSingle(), FADE_DURATION).From((!isOpen).ToSingle());
    }

    private static void OnPop(UIPanel panel, Tween tween, bool isOpen)
    {
        panel.PivotOffset = panel.Size / 2f;

        if (isOpen) tween.TweenPopIn(panel, POP_DURATION);
        else tween.TweenPopOut(panel, POP_DURATION);
    }

    private static void OnSlide(UIPanel panel, Tween tween, bool isOpen, Vector2 direction)
    {
        if (!defaultPositions.TryGetValue(panel, out Vector2 defaultPos))
        {
            defaultPos = panel.Position;
            defaultPositions[panel] = defaultPos;
        }

        Vector2 startPos = isOpen ? defaultPos + direction * SLIDE_AMOUNT : defaultPos;
        Vector2 endPos = isOpen ? defaultPos : defaultPos + direction * SLIDE_AMOUNT;

        tween.SetParallel();
        tween.TweenFade(panel, isOpen.ToSingle(), SLIDE_DURATION).From((!isOpen).ToSingle());
        tween.TweenPosition(panel, endPos, SLIDE_DURATION).From(startPos);
    }

    private static void DummyMethod() { }
}

