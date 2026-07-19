namespace Root.Systems;

using Godot;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Core;
using fennecs;
using SolFramework.MoveManager;


public partial class ComputeVelocity : Node, ISystem
{
	private static readonly World world = Core.World;
	
	public int Priority => SPriority.Transformation + 10;
	
	public void Process(double _)
	{
		ResetVelocity();
		ApplyMoveVelocity();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
	
	private static readonly Stream<Velocity> toReset =
		world.Query<Velocity>()
			.Has<MoveVelocity>()
			.Stream();
	private void ResetVelocity()
	{
		toReset.For(static(ref Velocity vel) =>
		{
			vel.Value = Vector2.Zero;
		});
	}
	
	private static readonly Stream<Velocity, MoveVelocity> toApplyMoveVel =
		world.Query<Velocity, MoveVelocity>()
			.Stream();
	private static void ApplyMoveVelocity()
	{
		toApplyMoveVel.For(
			static (ref Velocity vel, ref MoveVelocity moveVel) =>
			{
				vel.Value += moveVel.Value;
			});
	}
}