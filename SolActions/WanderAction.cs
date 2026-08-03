namespace SolActions;

using Godot;
using fennecs;
using GodotUtilities;
using SolFramework;
using SolFramework.Tools;
using SolFramework.UtilityAI;
using SolFramework.UtilityAI.Components;

public record struct WanderRadius(float Value);
public record struct WanderCooldown(TickTimer Value);
public record struct WanderGoal(Vector2 Value);
public record struct WanderRached;
public record struct WanderFinished;

public record struct Wandering;

public class WanderAction : BaseAction
{
	public static readonly float WANDER_RADIUS = 200f;
	private static readonly float score = 0.2f;
	
	public override bool CanRun(Entity entity) =>
		entity.HasAll<WanderCooldown, EcsCharBody2D>();

	public override float Score(Entity entity)
	{
		if (entity.Has<WanderGoal>() | entity.Ref<WanderCooldown>().Value.JustFinished())
			return score;
		else
			return 0f;
	}

	public override void Start(Entity entity)
	{
		entity.Add<Wandering>();
	}

	public override void Stop(Entity entity)
	{
		entity.Ref<WanderCooldown>().Value.Reset();
		entity.Remove<Wandering>();
		
		entity.TryRemove<WanderGoal>();
	}
}