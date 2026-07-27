namespace SolFramework;

using Godot;

public static class SolRandom
{
	public static Vector2 RandVec2Radius(float radius)
	{
		if (radius <= 0f)
            return Vector2.Zero;
		
		float angle = (float)GD.RandRange(0f, Mathf.Tau);
		float r = radius * Mathf.Sqrt(GD.Randf());
		return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
	}
	
	public static Vector2 RandVec2Box(Vector2 min, Vector2 max) =>
		new Vector2(
			(float)GD.RandRange(min.X, max.X),
			(float)GD.RandRange(min.Y, max.Y)
		);
	public static Vector2 RandVec2Box(Vector2 distance) => RandVec2Box(-distance, distance);
	
}