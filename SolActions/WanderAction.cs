namespace SolActions;

using fennecs;
using GodotUtilities;
using SolFramework;
using SolFramework.Tools;
using SolFramework.UtilityAI;
using SolFramework.UtilityAI.Components;

public record struct WanderRange(float Value);
public record struct WanderCooldown(TickTimer Value);

public record struct Wandering;

public class WanderAction : BaseAction
{
	public static readonly float WANDER_RANGE = 100f;
	private static readonly float score = 0.2f;
	
	public override bool CanExecute(Entity entity) =>
		entity.Has<WanderCooldown>();

	public override float Score(Entity entity)
	{
		if (entity.Ref<WanderCooldown>().Value.JustFinished())
			return score;
		else
			return 0f;
	}

	public override void Start(Entity entity)
	{
		entity.TryAdd<Wandering>();
	}

	public override void Stop(Entity entity)
	{
		entity.Ref<WanderCooldown>().Value.Reset();
		entity.Remove<Wandering>();
	}
}