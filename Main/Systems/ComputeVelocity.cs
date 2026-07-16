namespace Main.Systems;

using Godot;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Core;
using fennecs;
using SolFramework.MoveManager;

public partial class ComputeVelocity : Node, ISystem
{
	private static World world = Core.World;
	
	public int Priority => SPriority.Transformation + 10;
	
	public void Process(double _)
	{
		_resetVelocity();
		_applyMoveVel();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
	
	private static Stream<Velocity> toReset =
		world.Query<Velocity>()
		.Has<MoveVelocity>()
		.Stream();
	private void _resetVelocity()
	{
		toReset.Batch()
			.Remove<Velocity>()
			.Add(new Velocity(Vector2.Zero));
	}
	
	private static Stream<Velocity, MoveVelocity> toApplyMoveVel =
		world.Query<Velocity, MoveVelocity>()
			.Stream();
	private void _applyMoveVel()
	{
		toApplyMoveVel.For(
			static (ref Velocity vel, ref MoveVelocity moveVel) =>
				vel.Value += moveVel.Value
			);
			
	}
}