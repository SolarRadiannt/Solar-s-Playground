namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.Managers;
using SolFramework.Components;
public partial class FootstepsEmitter : Node, ISystem
{
	public int Priority => SPriority.Action + 1;
	public void Process(double delta)
	{
		ProcessFootstep(delta);
	}
	
	public void Init()
	{
		GD.Print("footstep sounds initialized");
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
	
	private static readonly World world = Core.World;
	private static readonly Stream<EcsCharBody2D, FootstepTimer, FootstepStride, ActualSpeed> toProcess =
		world.Query<EcsCharBody2D, FootstepTimer, FootstepStride, ActualSpeed>()
			.Has<Moving>()
			.Has<Grounded>()
			.Stream();
	private static void ProcessFootstep(double delta) =>
		toProcess.For(
			delta,
			static (
				double delta,
				in Entity entity,
				ref EcsCharBody2D body,
				ref FootstepTimer footstepTimer,
				ref FootstepStride stride,
				ref ActualSpeed speed
			) => {
				var timer = footstepTimer.Value;
				timer.Duration = stride.Value;
				timer.Tick(delta * speed.Value);
				footstepTimer.Value = timer;
				
				if (timer.JustFinished())
					FootstepManager.EmitFootstep(body.GlobalPosition, "Unknown", entity);
			});
}