namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework;
using SolFramework.Components;
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
		GD.Print("Aply velocity initialized");
		Scheduler.RegisterSystem(this);
	}

    public override void _Ready() => Init();

	private static readonly World world = Core.World;
	private static readonly Stream<EcsCharBody2D, Velocity> toApplyVelocities =
		world.Stream<EcsCharBody2D, Velocity>();
	private static void ApplyVelocities()
	{
		toApplyVelocities.For(
			static (in Entity entity, ref EcsCharBody2D body, ref Velocity vel) =>
			{
				body.Velocity = vel.Value;
			});
	}
}