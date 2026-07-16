namespace SolFramework.MoveManager;

using fennecs;
using Godot;
using SolFramework.Components;


public record struct MoveSpeed(float Value);
public record struct MoveDirection(Vector2 Value);
public record struct MoveGoal(Vector2 Value);
public record struct MoveVelocity(Vector2 Value) : IVelocity;
public record struct MoveToReach(float Value);

public static class MoveManager
{
	public const float MOVETO_REACH = 0.5f;
	public static bool MoveTo(Entity entity, Vector2 goal)
	{
		bool overriden = false;
		if (entity.Has<MoveGoal>())
		{
			entity.Remove<MoveGoal>();
			overriden = true;
		}
		
		entity.Add(new MoveGoal(goal));
		return overriden;
	}
	public static Vector2 GetMoveDirection(Entity entity)
	{
		var dir = entity.Ref<MoveDirection>().Value;
		return dir.IsNormalized() ? dir : dir.Normalized();
	}
	public static float GetMoveToReach(Entity entity)
	{
		return entity.Has<MoveToReach>()
			? entity.Ref<MoveToReach>().Value
			: MOVETO_REACH;
	}
	
	public static bool StopMove(Entity entity)
	{
		if (!entity.Has<MoveGoal>())
			return false;
		
		entity.Remove<MoveGoal>();
		return true;
	}
}