using System;
using Godot;
namespace GodotUtilities;

public static class MathUtil
{
    private static RandomNumberGenerator RNG { get; } = new();

    static MathUtil() => RNG.Randomize();
    
    #region Exponential Lerp (frame-independent)

    /// <summary>
    /// Smoothly interpolates <paramref name="from"/> toward <paramref name="to"/> in a
    /// frame-rate-independent way using exponential decay.<br/>
    /// Unlike a fixed <c>Lerp</c>, the result is consistent regardless of delta time fluctuations.
    /// </summary>
    /// <param name="weight">Controls the speed of smoothing. Higher values = faster response.</param>
    public static float DeltaLerp(float from, float to, float dt, float weight)
    {
        float smoothing = 1f - Mathf.Exp(-weight * dt);
        return Mathf.Lerp(from, to, smoothing);
    }

    /// <inheritdoc cref="DeltaLerp(float, float, float, float)"/>
    public static Vector2 DeltaLerp(Vector2 from, Vector2 to, float dt, float weight)
    {
        float smoothing = 1f - Mathf.Exp(-weight * dt);
        return from.Lerp(to, smoothing);
    }

    #endregion

    #region Vector2 Random

    /// <summary>Returns a random unit vector (direction) with uniform angular distribution.</summary>
    public static Vector2 RandomUnit()
    {
        float angle = RNG.Randf() * Mathf.Tau;
        return Vector2.FromAngle(angle);
    }

    /// <summary>Returns a random point uniformly distributed inside <paramref name="rect"/>.</summary>
    public static Vector2 RandomInRect(Rect2 rect)
    {
        return new Vector2(
            RNG.RandfRange(rect.Position.X, rect.End.X),
            RNG.RandfRange(rect.Position.Y, rect.End.Y)
        );
    }

    /// <summary>Returns a random point on the circumference of a circle with the given <paramref name="radius"/>.</summary>
    public static Vector2 RandomOnCircle(float radius) => RandomUnit() * radius;

    /// <summary>
    /// Returns a random point uniformly distributed inside a circle with the given <paramref name="radius"/>.<br/>
    /// Uses sqrt-distributed radius sampling to avoid clustering near the center.
    /// </summary>
    public static Vector2 RandomInCircle(float radius)
    {
        float r = Mathf.Sqrt(RNG.Randf()) * radius;
        return RandomUnit() * r;
    }

    /// <summary>
    /// Returns an array of <paramref name="count"/> evenly spaced points on a circle with the given <paramref name="radius"/>.
    /// </summary>
    public static Vector2[] PointsOnCircle(float radius, int count)
    {
        var points = new Vector2[count];
        float step = Mathf.Tau / count;
        for (int i = 0; i < count; i++)
            points[i] = Vector2.FromAngle(i * step) * radius;
        return points;
    }

    #endregion

    #region Luck

    /// <summary>Returns <see langword="true"/> or <see langword="false"/> with equal probability.</summary>
    public static bool CoinFlip() => (int)RNG.Randi() % 2 == 0;

    /// <summary>
    /// Returns <see langword="true"/> with the given <paramref name="probability"/>.<br/>
    /// A value of <c>1.0</c> always returns <see langword="true"/>;
    /// <c>0.0</c> always returns <see langword="false"/>.
    /// </summary>
    public static bool Chance(float probability) => RNG.Randf() < probability;

    /// <summary>Returns a random element from <paramref name="items"/>.</summary>
    /// <exception cref="ArgumentException">Thrown if <paramref name="items"/> is empty.</exception>
    public static T PickRandom<T>(params T[] items)
    {
        if (items.Length == 0)
            throw new ArgumentException("Array is empty");
        return items[RNG.RandiRange(0, items.Length - 1)];
    }

    #endregion

    #region Additional Math

    /// <summary>Clamps <paramref name="value"/> to the range [0, 1].</summary>
    public static float Clamp01(float value) => Mathf.Clamp(value, 0f, 1f);

    /// <inheritdoc cref="Clamp01(float)"/>
    public static double Clamp01(double value) => Mathf.Clamp(value, 0.0, 1.0);

    /// <summary>
    /// Returns <paramref name="value"/> divided by <paramref name="length"/>, clamped to [0, 1].<br/>
    /// Useful for converting a distance or progress value into a normalized [0, 1] range.
    /// </summary>
    public static float Normalize(float value, float length) => Clamp01(value / length);

    /// <inheritdoc cref="Normalize(float, float)"/>
    public static double Normalize(double value, double length) => Clamp01(value / length);

    #endregion

    #region Random Range

    /// <summary>Returns a random <see langword="float"/> between <paramref name="min"/> and <paramref name="max"/> (inclusive).</summary>
    public static float RandfRange(float min, float max) => RNG.RandfRange(min, max);

    /// <summary>Returns a random <see langword="int"/> between <paramref name="min"/> and <paramref name="max"/> (inclusive).</summary>
    public static int RandRange(int min, int max) => RNG.RandiRange(min, max);

    #endregion

    #region Offscreen Spawn

    /// <summary>
    /// Returns a random point just outside the viewport of <paramref name="cam"/>, offset outward by <paramref name="margin"/>.<br/>
    /// Useful for spawning enemies or projectiles that enter from off-screen.
    /// </summary>
    public static Vector2 OffscreenSpawn(Camera2D cam, float margin)
    {
        return RandomOutsideRectPoint(cam.GetViewportRect(), margin);
    }

    /// <summary>
    /// Returns a random point on one of the four edges of <paramref name="rect"/> expanded by <paramref name="margin"/>.<br/>
    /// Each edge is equally likely to be chosen.
    /// </summary>
    public static Vector2 RandomOutsideRectPoint(Rect2 rect, float margin)
    {
        Rect2 expanded = rect.Grow(margin);
        int side = (int)RNG.Randi() % 4;
        return side switch
        {
            0 => new Vector2(expanded.Position.X, RNG.RandfRange(expanded.Position.Y, expanded.End.Y)),
            1 => new Vector2(expanded.End.X,      RNG.RandfRange(expanded.Position.Y, expanded.End.Y)),
            2 => new Vector2(RNG.RandfRange(expanded.Position.X, expanded.End.X), expanded.Position.Y),
            _ => new Vector2(RNG.RandfRange(expanded.Position.X, expanded.End.X), expanded.End.Y),
        };
    }

    #endregion
}

