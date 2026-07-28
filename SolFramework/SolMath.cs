namespace SolFramework;

using System;
using Godot;

/// <summary>
/// Provides a collection of general-purpose mathematical utility functions for interpolation,
/// normalization, and curve shaping. While these methods are particularly useful for utility-based AI
/// (e.g., scoring and evaluating factors), they can be applied in many other contexts such as animation,
/// signal processing, or data transformation.
/// </summary>
public static class SolMath
{
    /// <summary>
    /// Normalizes a value to a [0,1] range given a minimum and maximum.
    /// </summary>
    /// <param name="value">The input value to normalize.</param>
    /// <param name="min">The lower bound of the input range.</param>
    /// <param name="max">The upper bound of the input range.</param>
    /// <returns>
    /// A clamped value between 0 and 1, representing the relative position of <paramref name="value"/>
    /// within the range [<paramref name="min"/>, <paramref name="max"/>]. Returns 0 if <paramref name="max"/> &lt;= <paramref name="min"/>.
    /// </returns>
    public static float Normalize(float value, float min, float max)
    {
        if (max <= min) return 0f;
        return Mathf.Clamp((value - min) / (max - min), 0f, 1f);
    }

    /// <summary>
    /// Computes the inverse (complement) of the normalized value, i.e., 1 - Normalize(value, min, max).
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="min">The lower bound of the input range.</param>
    /// <param name="max">The upper bound of the input range.</param>
    /// <returns>A value in [0,1] that is the inverse of the normalized position.</returns>
    public static float InverseNormalize(float value, float min, float max) =>
        1f - Normalize(value, min, max);

    /// <summary>
    /// Normalizes the input and then squares the result, producing a quadratic curve.
    /// </summary>
    /// <remarks>
    /// This function applies a power of two to the normalized value, which gives a parabolic shape
    /// that starts slowly and accelerates (or vice versa depending on interpretation).
    /// </remarks>
    /// <param name="value">The input value.</param>
    /// <param name="min">The lower bound of the input range.</param>
    /// <param name="max">The upper bound of the input range.</param>
    /// <returns>The squared normalized value, clamped to [0,1].</returns>
    public static float NormalizeSquared(float value, float min, float max)
    {
        float x = Normalize(value, min, max);
        return x * x;
    }

    /// <summary>
    /// Normalizes the input and then takes the square root of the result, producing a square-root curve.
    /// </summary>
    /// <remarks>
    /// This function applies a square root to the normalized value, which gives a rapid initial increase
    /// that gradually flattens out.
    /// </remarks>
    /// <param name="value">The input value.</param>
    /// <param name="min">The lower bound of the input range.</param>
    /// <param name="max">The upper bound of the input range.</param>
    /// <returns>The square root of the normalized value, clamped to [0,1].</returns>
    public static float NormalizeSqrt(float value, float min, float max)
    {
        float x = Normalize(value, min, max);
        return Mathf.Sqrt(x);
    }

    /// <summary>
    /// Applies a smoothstep (Hermite) interpolation to the normalized input.
    /// </summary>
    /// <remarks>
    /// This function uses the classic smoothstep formula: 3x² - 2x³, which provides smooth easing
    /// at both ends of the [0,1] interval. Commonly used for animation and blending.
    /// </remarks>
    /// <param name="value">The input value.</param>
    /// <param name="min">The lower bound of the input range.</param>
    /// <param name="max">The upper bound of the input range.</param>
    /// <returns>The smoothstep interpolated value, clamped to [0,1].</returns>
    public static float Smoothstep(float value, float min, float max)
    {
        float x = Normalize(value, min, max);
        return x * x * (3f - 2f * x);
    }

    /// <summary>
    /// Computes a bell-shaped (parabolic) curve based on the distance from a center point.
    /// </summary>
    /// <remarks>
    /// The result is 1.0 when <paramref name="value"/> equals <paramref name="center"/>, and decreases
    /// quadratically to 0 as the distance reaches <paramref name="width"/>. Values outside the width
    /// are clamped to 0. This is useful for creating radial falloff or utility functions that peak
    /// at a target value.
    /// </remarks>
    /// <param name="value">The input value.</param>
    /// <param name="center">The center point where the curve peaks.</param>
    /// <param name="width">The distance from center at which the curve reaches zero.</param>
    /// <returns>A value between 0 and 1 representing the bell curve response.</returns>
    public static float BellCurve(float value, float center, float width)
    {
        float x = Mathf.Abs(value - center) / width;
        return Mathf.Clamp(1f - x * x, 0f, 1f); // Simple inverted parabola
    }

    /// <summary>
    /// Computes a weighted sum of multiple score-weight pairs and clamps the result to [0,1].
    /// </summary>
    /// <remarks>
    /// This method is particularly useful for combining multiple utility factors in AI decision-making,
    /// where each factor contributes a score with an associated weight. The total is clamped to ensure
    /// it stays within a valid range.
    /// </remarks>
    /// <param name="factors">An array of tuples, each containing a score and a weight.</param>
    /// <returns>
    /// The weighted sum of all factors, clamped between 0 and 1. If no factors are provided, returns 0.
    /// </returns>
    public static float WeightedSum(params (float score, float weight)[] factors)
    {
        float total = 0f;
        foreach (var factor in factors) total += factor.score * factor.weight;
        return Math.Clamp(total, 0f, 1f);
    }

    /// <summary>
    /// Computes a Gaussian (normal) distribution curve centered at a given mean with specified standard deviation.
    /// </summary>
    /// <remarks>
    /// The output is 1.0 at the mean and decays smoothly as the distance from the mean increases.
    /// This is often used in utility AI to model preferences for an exact target value with a smooth,
    /// probabilistic falloff.
    /// </remarks>
    /// <param name="value">The input value.</param>
    /// <param name="mean">The center (peak) of the Gaussian curve.</param>
    /// <param name="sigma">The standard deviation, controlling the width of the bell.</param>
    /// <returns>A value in (0,1] representing the Gaussian response.</returns>
    public static float Gaussian(float value, float mean, float sigma)
    {
        float diff = value - mean;
        return Mathf.Exp(-(diff * diff) / (2f * sigma * sigma));
    }

    /// <summary>
    /// Computes the logistic sigmoid function, producing an S‑shaped curve.
    /// </summary>
    /// <remarks>
    /// The output approaches 0 for very low inputs and 1 for very high inputs, with a smooth transition
    /// around the <paramref name="midpoint"/>. This is useful for binary classification, threshold
    /// decisions, or creating smooth "on/off" responses in AI.
    /// </remarks>
    /// <param name="value">The input value.</param>
    /// <param name="midpoint">The x‑value where the output equals 0.5 (default 0).</param>
    /// <param name="steepness">Controls the slope of the transition; higher values make the curve steeper (default 1).</param>
    /// <returns>A value between 0 and 1.</returns>
    public static float Sigmoid(float value, float midpoint = 0f, float steepness = 1f) =>
        1f / (1f + Mathf.Exp(-steepness * (value - midpoint)));

    /// <summary>
    /// Computes exponential decay, often used for diminishing returns or urgency scaling.
    /// </summary>
    /// <remarks>
    /// The function returns 1 at <paramref name="value"/> = 0 and asymptotically approaches 0 as
    /// <paramref name="value"/> increases. The decay rate is controlled by <paramref name="rate"/>.
    /// Higher rates cause faster decay. This can model factors like "urgency decreases over time"
    /// or "distance falloff".
    /// </remarks>
    /// <param name="value">The input (usually non‑negative).</param>
    /// <param name="rate">The decay rate (default 1). Larger values produce quicker decay.</param>
    /// <returns>A value in (0,1] decreasing with <paramref name="value"/>.</returns>
    public static float ExponentialDecay(float value, float rate = 1f) => Mathf.Exp(-rate * value);

    /// <summary>
    /// Applies a power‑based ease‑in curve to a normalized input.
    /// </summary>
    /// <remarks>
    /// The curve starts slowly and accelerates toward the end. This is useful for scoring functions
    /// where small increases in input yield little response initially, but larger inputs have a growing impact.
    /// The input <paramref name="t"/> is expected to be in [0,1]; values outside may produce unexpected results.
    /// </remarks>
    /// <param name="t">The normalized time or input (typically 0–1).</param>
    /// <param name="power">The exponent controlling the degree of easing (default 2 for quadratic).</param>
    /// <returns>The eased value.</returns>
    public static float EaseIn(float t, float power = 2f) => Mathf.Pow(t, power);

    /// <summary>
    /// Applies a power‑based ease‑out curve to a normalized input.
    /// </summary>
    /// <remarks>
    /// The curve starts quickly and decelerates toward the end. This is the inverse of <see cref="EaseIn"/>.
    /// Useful when you want a strong initial response that gradually tapers off.
    /// The input <paramref name="t"/> is expected to be in [0,1].
    /// </remarks>
    /// <param name="t">The normalized time or input (typically 0–1).</param>
    /// <param name="power">The exponent (default 2 for quadratic).</param>
    /// <returns>The eased value.</returns>
    public static float EaseOut(float t, float power = 2f) => 1f - Mathf.Pow(1f - t, power);

    /// <summary>
    /// Applies a power‑based ease‑in‑out curve to a normalized input.
    /// </summary>
    /// <remarks>
    /// The curve combines <see cref="EaseIn"/> for the first half and <see cref="EaseOut"/> for the second half,
    /// producing a smooth acceleration then deceleration. This is ideal for animations or blending that require
    /// a natural, symmetric easing.
    /// The input <paramref name="t"/> is expected to be in [0,1].
    /// </remarks>
    /// <param name="t">The normalized time or input (typically 0–1).</param>
    /// <param name="power">The exponent (default 2 for quadratic).</param>
    /// <returns>The eased value.</returns>
    public static float EaseInOut(float t, float power = 2f) =>
        t < 0.5f ? Mathf.Pow(t * 2f, power) / 2f : 1f - Mathf.Pow((1f - t) * 2f, power) / 2f;
}