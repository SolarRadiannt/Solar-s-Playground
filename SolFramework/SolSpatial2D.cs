namespace SolFramework;

using System.Linq;
using Godot;
using Mapster;
using Root;

public struct SolPointQuery()
{
	public uint CollisionMask = uint.MaxValue;
	public bool CollideWithAreas = true;
	public bool CollideWithBodies = true;
	public ulong? CanvasInstanceId = null;
	public int MaxResults = 32;
	public Rid[] Exclude = System.Array.Empty<Rid>();
}

public struct SolPointHit
{
	public GodotObject Collider;
	public Rid Rid;
	public int ColliderId;
	public int ShapeIdx;
}

public static class SolSpatial2D
{
	public static SolPointHit[] IntersectPoint(Vector2 point, SolPointQuery parameters)
	{
		var spaceState = MainGame.World2D.DirectSpaceState;
		var query = new PhysicsPointQueryParameters2D
		{
			Position = point,
			CollideWithAreas = parameters.CollideWithAreas,
			CollisionMask = parameters.CollisionMask,
			CollideWithBodies = parameters.CollideWithBodies,
			CanvasInstanceId = parameters.CanvasInstanceId ?? 0,
			Exclude = [.. parameters.Exclude],
		};
		var results = spaceState.IntersectPoint(query, parameters.MaxResults);
		var processedResults = new SolPointHit[results.Count];
		
		for (int i = 0; i < results.Count; i++)
		{
			var data = results[i];
			processedResults[i] = new SolPointHit
			{
				Collider = (GodotObject)data["collider"],
				ColliderId = (int)data["collider_id"],
				Rid = (Rid)data["rid"],
				ShapeIdx = (int)data["shape"],
			};
		}
		
		return processedResults;
	}
}