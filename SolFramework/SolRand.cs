namespace SolFramework;

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// A static utility class for generating random values using a shared <see cref="RandomNumberGenerator"/>.
/// All methods are thread‑safe as long as they are not called concurrently from multiple threads
/// (the underlying RNG is not thread‑safe).
/// </summary>
public static class SolRand
{
	private static readonly Color MIN_COLOR = new(0f, 0f, 0f, 1f);
	private static readonly Color MAX_COLOR = new(1f, 1f, 1f, 1f);
	private static readonly RandomNumberGenerator _rng = new();
    /// <summary>
    /// Sets the seed for the random number generator, making all subsequent results deterministic.
    /// </summary>
    /// <param name="seed">The seed value (unsigned 64‑bit).</param>
    public static void SetSeed(ulong seed) => _rng.Seed = seed;

    /// <inheritdoc cref="SetSeed(ulong)"/>
    public static void SetSeed(int seed) => SetSeed((ulong)seed);

	/// <summary>
    /// Returns a random float between <paramref name="min"/> and <paramref name="max"/> (inclusive).
    /// </summary>
    /// <param name="min">Lower bound (inclusive).</param>
    /// <param name="max">Upper bound (inclusive).</param>
    /// <returns>A uniformly distributed float in [min, max].</returns>
	public static float Float(float min, float max) => (float)_rng.RandfRange(min, max);

	/// <summary>
    /// Returns a random float in the range [0, 1) (i.e. 0 inclusive, 1 exclusive).
    /// </summary>
	public static float Float() => _rng.Randf();

	/// <summary>
    /// Returns true with a probability of 50%.
    /// </summary>
	public static bool Flip() => Float() < 0.5f;

	/// <summary>
    /// Returns a random integer between <paramref name="min"/> and <paramref name="max"/> (inclusive).
    /// </summary>
    /// <param name="min">Lower bound (inclusive).</param>
    /// <param name="max">Upper bound (inclusive).</param>
    /// <returns>A uniformly distributed integer in [min, max].</returns>
	public static int Int(int min, int max) => _rng.RandiRange(min, max);

	/// <summary>
    /// Returns a random point uniformly distributed inside a circle (disk) of the given <paramref name="radius"/>.
    /// </summary>
    /// <param name="radius">The radius of the circle. If zero or negative, returns <see cref="Vector2.Zero"/>.</param>
    /// <returns>A point inside the disk, with distribution proportional to area (not angularly biased).</returns>
    /// <remarks>
    /// Uses the standard method: angle uniform in [0, 2π] and radius proportional to sqrt(random) to ensure uniform area distribution.
    /// </remarks>
	public static Vector2 Vec2Radius(float radius)
	{
		if (radius <= 0f)
            return Vector2.Zero;
		
		float angle = Float(0f, Mathf.Tau);
		float r = radius * Mathf.Sqrt(Float());
		return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
	}
	
	/// <summary>
    /// Returns a random point uniformly distributed inside an axis‑aligned rectangle defined by <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="min">The lower‑left corner of the rectangle (inclusive).</param>
    /// <param name="max">The upper‑right corner of the rectangle (inclusive).</param>
	public static Vector2 Vec2Box(Vector2 min, Vector2 max) => new(
		Float(min.X, max.X),
		Float(min.Y, max.Y)
	);

	/// <summary>
    /// Returns a random point uniformly distributed inside a square box centered at the origin,
    /// with half‑extents given by <paramref name="distance"/> (i.e. from -distance to +distance on each axis).
    /// </summary>
	public static Vector2 Vec2Box(Vector2 distance) => Vec2Box(-distance, distance);

	/// <summary>
    /// Returns a random point uniformly on the circumference of a circle of the given <paramref name="radius"/>.
    /// </summary>
	public static Vector2 Vec2RadiusEdge(float radius) => Vec2Direction() * radius;
	public static Vector2 Vec2Direction() => new(
		Mathf.Cos(Float(0, Mathf.Tau)),
		Mathf.Sin(Float(0, Mathf.Tau))
	);

	/// <summary>
    /// Returns a random point uniformly distributed inside an annulus (ring) between <paramref name="innerRadius"/> and <paramref name="outerRadius"/>.
    /// </summary>
    /// <param name="innerRadius">The inner radius (must be ≥ 0).</param>
    /// <param name="outerRadius">The outer radius (must be ≥ innerRadius).</param>
    /// <returns>A point uniformly distributed by area within the annulus.</returns>
    /// <remarks>
    /// The radius is chosen with a sqrt of a uniform value between innerRadius² and outerRadius² to ensure uniform area distribution.
    /// </remarks>
	public static Vector2 Vec2Annulus(float innerRadius, float outerRadius)
	{
		float angle = Float(0, Mathf.Tau);
		float r = Mathf.Sqrt(Float(innerRadius * innerRadius, outerRadius * outerRadius));
		return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
	}

	/// <summary>
    /// Returns a unit vector randomly spread around <paramref name="direction"/> within a cone of total width <paramref name="spreadAngle"/>.
    /// </summary>
    /// <param name="direction">The central direction (does not have to be unit length; only its angle is used).</param>
    /// <param name="spreadAngle">The full cone angle in radians. The offset is chosen uniformly in [-spreadAngle/2, spreadAngle/2].</param>
    /// <returns>A unit vector whose direction deviates from <paramref name="direction"/> by a random angle within the cone.</returns>
	public static Vector2 Vec2SpreadDir(Vector2 direction, float spreadAngle)
	{
		float baseAngle = direction.Angle();          // angle of the input vector
		float offset = Float(-spreadAngle * 0.5f, spreadAngle * 0.5f);
		float newAngle = baseAngle + offset;
		return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
	}

	/// <summary>
    /// Returns a vector with the same length as <paramref name="direction"/> but with its direction randomly spread
    /// within a cone of total width <paramref name="spreadAngle"/>.
    /// </summary>
    /// <param name="direction">The original vector (its direction is the centre of the cone).</param>
    /// <param name="spreadAngle">The full cone angle in radians.</param>
    /// <returns>A vector with the same length as <paramref name="direction"/> but with a randomly perturbed direction.</returns>
	public static Vector2 Vec2SpreadVec(Vector2 direction, float spreadAngle)
	{
		float length = direction.Length();
		return Vec2SpreadDir(direction, spreadAngle) * length;
	}

	/// <summary>
    /// Returns a unit vector randomly spread around <paramref name="direction"/> with a Gaussian (normal) distribution
    /// of angular deviation.
    /// </summary>
    /// <param name="direction">Central direction (only its angle is used).</param>
    /// <param name="stdDevAngle">The standard deviation of the angular offset in radians.</param>
    /// <returns>A unit vector with direction = baseAngle + N(0, stdDevAngle²).</returns>
    /// <remarks>
    /// The offset is sampled using the Box‑Muller transform via <see cref="RandNormal"/>.
    /// This produces a natural clustering around the centre, unlike the uniform spread.
    /// </remarks>
	public static Vector2 Vec2SpreadDirG(Vector2 direction, float stdDevAngle)
	{
		float baseAngle = direction.Angle();
		float offset = Normal(0f, stdDevAngle);
		float newAngle = baseAngle + offset;
		return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
	}

	/// <summary>
    /// Returns a random element from an array.
    /// </summary>
    /// <param name="array">The array to pick from (must not be null or empty).</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>A randomly chosen element.</returns>
	public static T Choice<T>(T[] array) => array[Int(0, array.Length - 1)];

	/// <summary>
    /// Returns a random element from a list.
    /// </summary>
    /// <param name="list">The list to pick from (must not be null or empty).</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>A randomly chosen element.</returns>
	public static T Choice<T>(List<T> list) => list[Int(0, list.Count - 1)];

	/// <summary>
    /// Returns a random float from a Gaussian (normal) distribution with the specified <paramref name="mean"/> and standard deviation.
    /// </summary>
    /// <param name="mean">The mean (average) of the distribution.</param>
    /// <param name="stdDev">The standard deviation (spread). Must be ≥ 0.</param>
    /// <returns>A normally distributed value.</returns>
    /// <remarks>
    /// Uses the Box‑Muller transform. The two uniform variables are generated from <see cref="Float()"/>.
    /// </remarks>
	public static float Normal(float mean = 0, float stdDev = 1)
	{
		float u1 = 1f - Float();
		float u2 = 1f - Float();
		float z = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(Mathf.Tau * u2);
		return mean + stdDev * z;
	}

	/// <summary>
    /// Returns a random colour with each component (R, G, B, A) uniformly distributed in [0, 1].
    /// </summary>
	public static Color Color() => Color(MIN_COLOR, MAX_COLOR);

	/// <summary>
    /// Returns a random colour with each component independently uniformly distributed between
    /// the corresponding component of <paramref name="min"/> and <paramref name="max"/> (inclusive).
    /// </summary>
    /// <param name="min">The minimum colour (each component).</param>
    /// <param name="max">The maximum colour (each component).</param>
    /// <remarks>
    /// It is assumed that min.R ≤ max.R, min.G ≤ max.G, etc. No clamping is performed.
    /// </remarks>
	public static Color Color(Color min, Color max) => new(
		Float(min.R, max.R),
		Float(min.G, max.G),
		Float(min.B, max.B),
		Float(min.A, max.A)
	);

	/// <summary>
    /// Randomly shuffles the elements of a list in place using the Fisher‑Yates algorithm.
    /// </summary>
    /// <param name="list">The list to shuffle (must not be null).</param>
    /// <typeparam name="T">The element type.</typeparam>
	public static void Shuffle<T>(IList<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = Int(0, i);
			(list[i], list[j]) = (list[j], list[i]);
		}
	}

	/// <summary>
    /// Selects an index from a weight array with probability proportional to each weight.
    /// </summary>
    /// <param name="weights">An array of non‑negative weights. If all weights are zero, the last index is returned.</param>
    /// <returns>The selected index.</returns>
    /// <remarks>
    /// The total weight is calculated by summing all entries. A random float in [0, total) is then used to
    /// walk through the weights until the cumulative sum exceeds that value.
    /// </remarks>
	public static int WeightedIndex(float[] weights)
	{
		float total = 0;
		foreach (var w in weights) total += w;
		float r = Float(0, total);
		for (int i = 0; i < weights.Length; i++)
		{
			r -= weights[i];
			if (r <= 0) return i;
		}
		return weights.Length - 1;
	}
	
}