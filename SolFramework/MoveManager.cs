namespace SolFramework.MoveManager;

using fennecs;
using Godot;

public record struct MoveSpeed(float Value);
public record struct MoveDirection(Vector2 Value);
public record struct MoveGoal(Vector2 Value);
public record struct MoveVelocity(Vector2 Value);

public static class MoveManager
{
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
	
	public static bool StopMove(Entity entity)
	{
		if (!entity.Has<MoveGoal>())
			return false;
		
		entity.Remove<MoveGoal>();
		return true;
	}
}