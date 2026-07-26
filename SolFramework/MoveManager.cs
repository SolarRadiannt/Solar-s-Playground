namespace SolFramework.Managers;

using fennecs;
using Godot;
using SolFramework.Tools;

[InspectorColor(0.5f, 0.5f, 0.2f)]
public record struct MoveSpeed(float Value);
public record struct MoveDirection(Vector2 Value);
public record struct MoveToGoal(Vector2 Value);
public record struct MoveVelocity(Vector2 Value);
public record struct MoveToReachDistance(float Value);
public record struct LookSpeed(float Value);

[InspectorColor(1f, 0f, 0f)]
public struct MovingBlocked;

[InspectorColor(0f, 1f, 0f)]
public struct Moving;
public struct LookAtMoveDir;

public static class MoveManager
{
	public const float MOVETO_REACH = 5f;
	
	public static void ApplyMovement(Entity entity, float moveSpeed) =>
		entity
			.Add(new MoveSpeed(moveSpeed))
			.Add(new MoveVelocity(Vector2.Zero))
			.Add(new MoveDirection(Vector2.Zero));
	public static void SetMoveDirection(Entity entity, Vector2 direction)
	{
		if (entity.Has<MoveDirection>())
			entity.Ref<MoveDirection>().Value = direction;
		else
			entity.Add(new MoveDirection(direction));
	}
	public static bool MoveTo(Entity entity, Vector2 goal)
	{
		if (entity.Has<MoveToGoal>())
		{
			entity.Ref<MoveToGoal>().Value = goal;
			return false;
		}
		else
		{
			entity.Add(new MoveToGoal(goal));
			return true;
		}
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