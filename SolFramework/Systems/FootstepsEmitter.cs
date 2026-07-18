namespace SolFramework.Systems;

using Godot;
using SolFramework.Scheduler;
using SolFramework.EEvents;
using SolFramework.TickTimer;
using SolFramework.Core;
using SolFramework.FootstepManager;
using fennecs;
using SolFramework.MoveManager;
using SolFramework.Components;

public partial class FootstepsEmitter : Node, ISystem
{
	public int Priority => SPriority.Action;
	public void Process(double delta)
	{
		ProcessFootstep(delta);
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
	
	private static World world = Core.World;
	private static Stream<ECSCharBody2D, FootstepTimer> toProcess =
		world.Query<ECSCharBody2D, FootstepTimer>()
			.Has<Moving>()
			.Has<Grounded>()
			.Stream();
	private static void ProcessFootstep(double delta) =>
		toProcess.For(
			delta,
			static (double delta, in Entity entity, ref ECSCharBody2D body, ref FootstepTimer footstepTimer) =>
			{
				var timer = footstepTimer.Value;
				if (entity.Has<MoveSpeed>())
				{
					float speed = entity.Ref<MoveSpeed>().Value;
					float stepRate = entity.Ref<FootstepBaseRate>().Value / Mathf.Max(speed, 1f);
					timer.Duration = stepRate;
				}
				
				timer.Tick(delta);
				footstepTimer.Value = timer;
				
				if (timer.JustFinished())
					FootstepManager.EmitFootstep(body.GlobalPosition, "Unknown", entity);
			});
}