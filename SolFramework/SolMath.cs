namespace SolFramework;

using System;
using Godot;

public static class SolMath
{
	public static float Normalize(float value, float min, float max)
	{
		if (max <= min) return 0f;
		return Mathf.Clamp((value - min) / (max - min), 0f, 1f);
	}
	public static float InverseNormalize(float value, float min, float max) =>
		1f - Normalize(value, min, max);
	
	
	public static float NormalizeSquared(float value, float min, float max)
	{
		float x = Normalize(value, min, max);
		return x * x;
	}

	public static float NormalizeSqrt(float value, float min, float max)
	{
		float x = Normalize(value, min, max);
		return Mathf.Sqrt(x);
	}
	
	public static float Smoothstep(float value, float min, float max)
	{
		float x = Normalize(value, min, max);
		return x * x * (3f - 2f * x);
	}
	
	public static float BellCurve(float value, float center, float width)
	{
		float x = Mathf.Abs(value - center) / width;
		return Mathf.Clamp(1f - x * x, 0f, 1f); // Simple inverted parabola
	}
	
	public static float WeightedSum(params (float score, float weight)[] factors)
	{
		float total = 0f;
		foreach (var factor in factors) total += factor.score * factor.weight;
		return Math.Clamp(total, 0f, 1f);
	}
}