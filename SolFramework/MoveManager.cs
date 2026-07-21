namespace SolFramework.MoveManager;

using fennecs;
using Godot;
using SolFramework.Components;


public record struct MoveSpeed(float Value);
public record struct MoveDirection(Vector2 Value);
public record struct MoveToGoal(Vector2 Value);
public record struct MoveVelocity(Vector2 Value);
public record struct MoveToReachDistance(float Value);
public struct MovingBlocked;
public struct Moving;

public static class MoveManager
{
	public const float MOVETO_REACH = 0.5f;
	
	public static Entity ApplyMovement(Entity entity, float moveSpeed) =>
		entity
			.Add(new MoveSpeed(moveSpeed))
			.Add(new MoveVelocity(Vector2.Zero))
			.Add(new MoveDirection(Vector2.Zero));
	
	public static bool MoveTo(Entity entity, Vector2 goal)
	{
		bool overriden = false;
		if (entity.Has<MoveToGoal>())
		{
			entity.Remove<MoveToGoal>();
			overriden = true;
		}
		
		entity.Add(new MoveToGoal(goal));
		return overriden;
	}
	
	public static bool MoveToActive(Entity entity) =>
		entity.Has<MoveToGoal>();
	
	public static Vector2 GetMoveDirection(Entity entity)
	{
		var dir = entity.Ref<MoveDirection>().Value;
		return dir.IsNormalized() ? dir : dir.Normalized();
	}
	public static float GetMoveToReach(Entity entity) =>
		entity.Has<MoveToReachDistance>()
			? entity.Ref<MoveToReachDistance>().Value
			: MOVETO_REACH;
	
	
	public static bool StopMove(Entity entity)
	{
		if (!entity.Has<MoveToGoal>())
			return false;
		
		entity.Remove<MoveToGoal>();
		return true;
	}
}