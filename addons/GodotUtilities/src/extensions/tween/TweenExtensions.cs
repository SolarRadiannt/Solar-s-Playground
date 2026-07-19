using System;
using Godot;

namespace GodotUtilities;

public static partial class TweenExtensions
{
    #region Cached Properties 

    private const string PROPERTY_POSITION         = "position";
    private const string PROPERTY_GLOBAL_POSITION  = "global_position";
    private const string PROPERTY_SCALE            = "scale";
    private const string PROPERTY_ROTATION_DEGREES = "rotation_degrees";
    private const string PROPERTY_ROTATION         = "rotation";
    private const string PROPERTY_MODULATE_ALPHA   = "modulate:a";
    private const string PROPERTY_MODULATE         = "modulate";
    private const string PROPERTY_SELF_MODULATE    = "self_modulate";
    private const string PROPERTY_COLOR            = "color";
    private const string PROPERTY_VISIBLE_RATIO    = "visible_ratio";
    private const string PROPERTY_VOLUME           = "volume_db";
    private const string PROPERTY_VOLUME_LINEAR    = "volume_linear";
    private const string PROPERTY_PROGRESS_RATIO   = "progress_ratio";
    private const string PROPERTY_SKEW             = "skew";
    private const string PROPERTY_OFFSET           = "offset";

    #endregion

    #region Transitions & Ease

    public static Tween Linear(this Tween tween)  => tween.SetTrans(Tween.TransitionType.Linear);
    public static Tween Sine(this Tween tween)    => tween.SetTrans(Tween.TransitionType.Sine);
    public static Tween Back(this Tween tween)    => tween.SetTrans(Tween.TransitionType.Back);
    public static Tween Bounce(this Tween tween)  => tween.SetTrans(Tween.TransitionType.Bounce);
    public static Tween Circ(this Tween tween)    => tween.SetTrans(Tween.TransitionType.Circ);
    public static Tween Spring(this Tween tween)  => tween.SetTrans(Tween.TransitionType.Spring);
    public static Tween Quad(this Tween tween)    => tween.SetTrans(Tween.TransitionType.Quad);
    public static Tween Quart(this Tween tween)   => tween.SetTrans(Tween.TransitionType.Quart);
    public static Tween Expo(this Tween tween)    => tween.SetTrans(Tween.TransitionType.Expo);
    public static Tween Quint(this Tween tween)   => tween.SetTrans(Tween.TransitionType.Quint);
    public static Tween Elastic(this Tween tween) => tween.SetTrans(Tween.TransitionType.Elastic);
    public static Tween Cubic(this Tween tween)   => tween.SetTrans(Tween.TransitionType.Cubic);

    public static Tween EaseIn(this Tween tween)    => tween.SetEase(Tween.EaseType.In);
    public static Tween EaseOut(this Tween tween)   => tween.SetEase(Tween.EaseType.Out);
    public static Tween EaseOutIn(this Tween tween) => tween.SetEase(Tween.EaseType.OutIn);
    public static Tween EaseInOut(this Tween tween) => tween.SetEase(Tween.EaseType.InOut);

    public static PropertyTweener Linear(this PropertyTweener t)  => t.SetTrans(Tween.TransitionType.Linear);
    public static PropertyTweener Sine(this PropertyTweener t)    => t.SetTrans(Tween.TransitionType.Sine);
    public static PropertyTweener Back(this PropertyTweener t)    => t.SetTrans(Tween.TransitionType.Back);
    public static PropertyTweener Bounce(this PropertyTweener t)  => t.SetTrans(Tween.TransitionType.Bounce);
    public static PropertyTweener Circ(this PropertyTweener t)    => t.SetTrans(Tween.TransitionType.Circ);
    public static PropertyTweener Spring(this PropertyTweener t)  => t.SetTrans(Tween.TransitionType.Spring);
    public static PropertyTweener Quad(this PropertyTweener t)    => t.SetTrans(Tween.TransitionType.Quad);
    public static PropertyTweener Quart(this PropertyTweener t)   => t.SetTrans(Tween.TransitionType.Quart);
    public static PropertyTweener Expo(this PropertyTweener t)    => t.SetTrans(Tween.TransitionType.Expo);
    public static PropertyTweener Quint(this PropertyTweener t)   => t.SetTrans(Tween.TransitionType.Quint);
    public static PropertyTweener Elastic(this PropertyTweener t) => t.SetTrans(Tween.TransitionType.Elastic);
    public static PropertyTweener Cubic(this PropertyTweener t)   => t.SetTrans(Tween.TransitionType.Cubic);

    public static PropertyTweener EaseIn(this PropertyTweener t)    => t.SetEase(Tween.EaseType.In);
    public static PropertyTweener EaseOut(this PropertyTweener t)   => t.SetEase(Tween.EaseType.Out);
    public static PropertyTweener EaseOutIn(this PropertyTweener t) => t.SetEase(Tween.EaseType.OutIn);
    public static PropertyTweener EaseInOut(this PropertyTweener t) => t.SetEase(Tween.EaseType.InOut);

    #endregion

    #region Awaiters

    // Await
    public static SignalAwaiter WaitToFinish(this Tween tween) => tween.ToSignal(tween, Tween.SignalName.Finished);
    public static SignalAwaiter WaitToFinish(this Tweener tween) => tween.ToSignal(tween, Tweener.SignalName.Finished);

    #endregion

    #region Signals

    private static readonly uint OneShotFlag = (uint)GodotObject.ConnectFlags.OneShot;

    public static Tween OnFinished(this Tween tween, Action action)
    {
        tween.Connect(Tween.SignalName.Finished, Callable.From(action), OneShotFlag);
        return tween;
    }

    public static T OnFinished<T>(this T tweener, Action action) where T : Tweener
    {
        tweener.Connect(Tweener.SignalName.Finished, Callable.From(action), OneShotFlag);
        return tweener;
    }

    #endregion

    #region Other

    public static void KillIfValid(this Tween tween)
    {
        if (GodotObject.IsInstanceValid(tween) && tween.IsValid()) 
            tween.Kill();
    }

    public static PropertyTweener SetCustomInterpolator(this PropertyTweener tweener, Curve curve) =>
        tweener.SetCustomInterpolator(Callable.From<float, float>(curve.SampleBaked));
    
    public static CallbackTweener TweenAction(this Tween tween, Action action) =>
        tween.TweenCallback(Callable.From(action));

    #endregion

    #region UI

    public static PropertyTweener TweenPopIn(this Tween tween, Control target, double duration) =>
        tween.TweenProperty(target, PROPERTY_SCALE, Vector2.One, duration).From(Vector2.Zero).Back().EaseOut();

    public static PropertyTweener TweenPopOut(this Tween tween, Control target, double duration) =>
        tween.TweenProperty(target, PROPERTY_SCALE, Vector2.Zero, duration).From(Vector2.One).Back().EaseIn();

    #endregion

    #region Follow Path

    public static PropertyTweener TweenFollowPath(this Tween tween, PathFollow2D follower, double duration) =>
        tween.TweenProperty(follower, PROPERTY_PROGRESS_RATIO, 1f, duration);

    #endregion

    #region Move

    public static PropertyTweener TweenPosition(this Tween tween, GodotObject target, Vector2 to, double duration) =>
        tween.TweenProperty(target, PROPERTY_POSITION, to, duration);

    public static PropertyTweener TweenGlobalPosition(this Tween tween, GodotObject target, Vector2 to, double duration) =>
        tween.TweenProperty(target, PROPERTY_GLOBAL_POSITION, to, duration);

    #endregion

    #region Rotation

    public static PropertyTweener TweenRotationDeg(this Tween tween, GodotObject target, float degrees, double duration) =>
        tween.TweenProperty(target, PROPERTY_ROTATION_DEGREES, degrees, duration);

    public static PropertyTweener TweenRotation(this Tween tween, GodotObject target, float rad, double duration) =>
        tween.TweenProperty(target, PROPERTY_ROTATION, rad, duration);

    #endregion

    #region Scale

    public static PropertyTweener TweenScale(this Tween tween, GodotObject target, Vector2 scale, double duration) =>
        tween.TweenProperty(target, PROPERTY_SCALE, scale, duration);

    public static PropertyTweener TweenScaleUniform(this Tween tween, GodotObject target, float value, double duration) =>
        tween.TweenProperty(target, PROPERTY_SCALE, Vector2.One * value, duration);

    #endregion

    #region Skew

    public static PropertyTweener TweenSkew(this Tween tween, Node2D target, float rad, double duration) =>
        tween.TweenProperty(target, PROPERTY_SKEW, rad, duration);

    #endregion

    #region Offset

    public static PropertyTweener TweenOffset(this Tween tween, GodotObject target, Vector2 value, double duration) =>
        tween.TweenProperty(target, PROPERTY_OFFSET, value, duration);
    
    #endregion

    #region Squish

    public enum SquashDirection { Up, Down }

    public static Tween TweenSquish(this Tween tween, GodotObject target, double duration, float ratio = 0.2f, SquashDirection dir = SquashDirection.Up)
    {
        double step = duration / 3.0;

        var stretch = dir == SquashDirection.Up
            ? new Vector2(1f - ratio, 1f + ratio)
            : new Vector2(1f + ratio, 1f - ratio);

        var compress = new Vector2(2f - stretch.X, 2f - stretch.Y); 

        tween.TweenProperty(target, PROPERTY_SCALE, stretch, step).Spring().EaseIn();
        tween.TweenProperty(target, PROPERTY_SCALE, compress, step).Spring().EaseIn();
        tween.TweenProperty(target, PROPERTY_SCALE, Vector2.One, step).Spring().EaseOut();

        return tween;
    }

    #endregion

    #region Look At

    public static MethodTweener TweenLookAtFollow(this Tween tween, Node2D target, Func<Vector2> point, double duration, float deltaSmooth = 0)
    {
        void interpolate(float t)
        {
            float angle = target.GlobalPosition.DirectionTo(point()).Angle();

            if (deltaSmooth > 0f)
                target.Rotation = Mathf.Lerp(target.Rotation, angle, deltaSmooth);
            else
                target.Rotation = angle;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }

    #endregion

    #region Fade

    public static PropertyTweener TweenFade(this Tween tween, CanvasItem target, float value, double duration) =>
        tween.TweenProperty(target, PROPERTY_MODULATE_ALPHA, value, duration);

    public static PropertyTweener TweenFadeIn(this Tween tween, CanvasItem target, double duration) =>
        tween.TweenFade(target, 1f, duration);

    public static PropertyTweener TweenFadeOut(this Tween tween, CanvasItem target, double duration) =>
        tween.TweenFade(target, 0f, duration);

    #endregion

    #region Color

    public static PropertyTweener TweenModulate(this Tween tween, CanvasItem target, Color color, double duration) =>
        tween.TweenProperty(target, PROPERTY_MODULATE, color, duration);

    public static PropertyTweener TweenSelfModulate(this Tween tween, CanvasItem target, Color color, double duration) =>
        tween.TweenProperty(target, PROPERTY_SELF_MODULATE, color, duration);

    public static PropertyTweener TweenColor(this Tween tween, GodotObject target, Color color, double duration) =>
        tween.TweenProperty(target, PROPERTY_COLOR, color, duration);

    #endregion

    #region Wiggle

    public static MethodTweener TweenWiggle(this Tween tween, Node2D target, float degrees, double duration, int cycles = 10)
    {
        float? start = null;

        void interpolate(float t)
        {
            start ??= target.RotationDegrees;

            float damping = Mathf.Exp(-5f * t);
            float wave = Mathf.Sin(t * Mathf.Pi * cycles * 2f);

            target.RotationDegrees = start.Value + wave * damping * degrees;

            if (t >= 1f) target.RotationDegrees = start.Value;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }

    public static MethodTweener TweenWiggle(this Tween tween, Control target, float degrees, double duration, int cycles = 10)
    {
        float? start = null;

        void interpolate(float t)
        {
            start ??= target.RotationDegrees;

            float damping = Mathf.Exp(-5f * t);
            float wave = Mathf.Sin(t * Mathf.Pi * cycles * 2f);

            target.RotationDegrees = start.Value + wave * damping * degrees;

            if (t >= 1f) target.RotationDegrees = start.Value;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }
    
    #endregion

    #region Blink

    public static Tween TweenBlink(this Tween tween, CanvasItem item, int blinks, double duration = 0.2, bool endVisible = true)
    {
        Color start = item.Modulate;
        item.Modulate = new Color(start, 1f);

        double step = duration / (blinks * 2);

        for (int i = 0; i < blinks; i++)
        {
            tween.TweenProperty(item, PROPERTY_MODULATE_ALPHA, 0f, step).EaseInOut();
            tween.TweenProperty(item, PROPERTY_MODULATE_ALPHA, 1f, step).EaseInOut();
        }

        tween.TweenProperty(item, PROPERTY_MODULATE_ALPHA, endVisible ? start.A : 0f, 0.001);
        return tween;
    }
    
    #endregion

    #region Typewriter

    public static PropertyTweener TweenTypewriter(this Tween tween, Label label, double duration) =>
        tween.TweenProperty(label, PROPERTY_VISIBLE_RATIO, 1f, duration).From(0f);

    #endregion

    #region Counter

    public static MethodTweener TweenCounter(this Tween tween, Label label, float from, float to, double duration) =>
        tween.TweenMethod(Callable.From<float>(value => label.Text = Mathf.RoundToInt(value).ToString()), from, to, duration);

    #endregion

    #region Shader

    public static PropertyTweener TweenShader(this Tween tween, ShaderMaterial material, string paramName, Variant value, double duration) =>
        tween.TweenProperty(material, $"shader_parameter/{paramName}", value, duration);

    #endregion

    #region Audio

    public static PropertyTweener TweenVolumeDB(this Tween tween, AudioStreamPlayer player, float db, double duration) =>
        tween.TweenProperty(player, PROPERTY_VOLUME, db, duration);

    public static PropertyTweener TweenVolumeLinear(this Tween tween, AudioStreamPlayer player, float linear, double duration) =>
        tween.TweenProperty(player, PROPERTY_VOLUME_LINEAR, linear, duration);

    #endregion

    #region Shake Position

    public static MethodTweener TweenShakePosition(this Tween tween, Control target, double duration, 
        float strength = 15f, float freq = 60f, float damping = 6f)
    {
        Vector2? start = null;

        void interpolate(float t)
        {
            start ??= target.Position;

            float x = Mathf.Sin(t * freq) * Mathf.Exp(-damping * t) * strength;
            float y = Mathf.Sin(t * freq * 1.37f) * Mathf.Exp(-damping * t) * strength * 0.4f;

            target.Position = start.Value + new Vector2(x, y);

            if (t >= 1f)
                target.Position = start.Value;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }

    public static MethodTweener TweenShakePosition(this Tween tween, Node2D target, double duration, 
        float strength = 15f, float freq = 60f, float damping = 6f)
    {
        Vector2? start = null;

        void interpolate(float t)
        {
            start ??= target.Position;

            float x = Mathf.Sin(t * freq) * Mathf.Exp(-damping * t) * strength;
            float y = Mathf.Sin(t * freq * 1.37f) * Mathf.Exp(-damping * t) * strength * 0.4f;

            target.Position = start.Value + new Vector2(x, y);

            if (t >= 1f) target.Position = start.Value;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }

    #endregion

    #region Shake Rotation

    public static MethodTweener TweenShakeRotation(this Tween tween, Node2D target, 
        float degrees, double duration)
    {
        float? start = null;
        float strength = Mathf.DegToRad(degrees);
        
        void interpolate(float t)
        {
            start ??= target.Rotation;

            float decay = 1f - t;
            float offset = (float)GD.RandRange(-1.0, 1.0) * strength * decay;

            target.Rotation = start.Value + offset;
            if (t >= 1f) target.Rotation = start.Value;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }

    public static MethodTweener TweenShakeRotation(this Tween tween, Control target, 
        float degrees, double duration)
    {
        float? start = null;
        float strength = Mathf.DegToRad(degrees);
        
        void interpolate(float t)
        {
            start ??= target.Rotation;

            float decay = 1f - t;
            float offset = (float)GD.RandRange(-1.0, 1.0) * strength * decay;

            target.Rotation = start.Value + offset;
            if (t >= 1f) target.Rotation = start.Value;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }

    #endregion

    #region Heart Beat

    public static MethodTweener TweenHeartBeat(this Tween tween, Node2D target, float strength = 0.25f, double duration = 0.8)
    {
        Vector2? startScale = null;

        void interpolate(float t)
        {
            startScale ??= target.Scale;

            float beat1 = Mathf.Exp(-Mathf.Pow((t - 0.20f) / 0.06f, 2));
            float beat2 = Mathf.Exp(-Mathf.Pow((t - 0.35f) / 0.05f, 2));

            float pulse = beat1 + beat2 * 0.8f;

            target.Scale = startScale.Value * (1f + pulse * strength);

            if (t >= 1f) target.Scale = startScale.Value;
        }

        return tween.TweenMethod(Callable.From<float>(interpolate), 0f, 1f, duration);
    }

    #endregion
}

