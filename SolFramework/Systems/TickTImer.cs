namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Core;
using SolFramework.TimerManager;
using SolFramework.Scheduler;

public partial class TickTimer : Node, ISystem
{
	public int Priority => SPriority.Applying;
	public void Process(double delta)
	{
		AutoTick(delta);
		
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

    public override void _Ready() => Init();

	private static readonly World world = Core.World;
	private static readonly Stream<TimerEntity> toTick =
		world.Query<TimerEntity>()
			.Has<TimerAutoTick>()
			.Not<TimerPaused>()
			.Stream();
	private static void AutoTick(double delta)
	{
		toTick.For(
			delta,
			static (double delta, in Entity entity, ref TimerEntity _) =>
			{
				TimerManager.Tick(entity, delta);
			});
	}
}