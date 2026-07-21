namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework.Scheduler;
using SolFramework.Core;
using SolFramework.FootstepManager;
using SolFramework.MoveManager;
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
	private static readonly Stream<ECSCharBody2D, FootstepTimer, FootstepStride> toProcess =
		world.Query<ECSCharBody2D, FootstepTimer, FootstepStride>()
			.Has<Moving>()
			.Has<Grounded>()
			.Stream();
	private static void ProcessFootstep(double delta) =>
		toProcess.For(
			delta,
			static (
				double delta,
				in Entity entity,
				ref ECSCharBody2D body,
				ref FootstepTimer footstepTimer,
				ref FootstepStride stride
			) => {
				var timer = footstepTimer.Value;
				if (entity.Has<MoveSpeed>())
				{
					float speed = entity.Ref<MoveSpeed>().Value;
					float stepRate = stride.Value / Mathf.Max(speed, 1f);
					timer.Duration = stepRate;
				}
				
				timer.Tick(delta);
				footstepTimer.Value = timer;
				
				if (timer.JustFinished())
					FootstepManager.EmitFootstep(body.GlobalPosition, "Unknown", entity);
			});
}