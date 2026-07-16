namespace SolFramework.MoveManager;

using fennecs;
using Godot;


public record struct MoveDirection(Vector2 Value);
public record struct MoveGoal(Vector2 Value);

public static class MoveManager
{
	public static void MoveTo(Entity entity, Vector2 goal)
	{
		if (entity.Has<MoveGoal>())
			entity.Remove<MoveGoal>();
		
		entity.Add(new MoveGoal(goal));
	}
}