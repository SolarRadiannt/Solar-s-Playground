namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Components;
using SolFramework.Core;
using SolFramework.MoveManager;
using SolFramework.Scheduler;

public partial class ApplyVelocity : Node, ISystem
{
	public int Priority => SPriority.Applying + 5;
	public void Process(double _)
	{
		ApplyVelocities();
	}
	
	
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

    public override void _Ready() => Init();

	private static readonly World world = Core.World;
	private static readonly Stream<ECSCharBody2D, Velocity> toApplyVelocities =
		world.Stream<ECSCharBody2D, Velocity>();
	private static void ApplyVelocities()
	{
		toApplyVelocities.For(
			static (in Entity entity, ref ECSCharBody2D body, ref Velocity vel) =>
			{
				body.Velocity = vel.Value;
				
				if (!entity.Has<Moving>() && vel.Value.Length() > 0.01)
					entity.Add<Moving>();
				else if (entity.Has<Moving>())
						entity.Remove<Moving>();
			});
	}
}