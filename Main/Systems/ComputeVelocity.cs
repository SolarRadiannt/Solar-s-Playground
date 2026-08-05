namespace Root.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;

public partial class ComputeVelocity : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Transformation;
	
	public void Process(double _)
	{
		toReset.For(ResetVelocity);
		toApplyMoveVel.For(ApplyMoveVelocity);
		toApplyMoveVelToNode2D.For(ApplyMoveVelocity);
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}

	public override void _Ready() => Init();
	private static readonly Stream<Velocity> toReset =
		world.Query<Velocity>()
			.Stream();
	private static void ResetVelocity(ref Velocity vel) => vel.Value = Vector2.Zero;

	private static readonly Stream<Velocity, MoveVelocity> toApplyMoveVelToNode2D =
		world.Query<Velocity, MoveVelocity>()
			.Has<EcsNode2D>()
			.Not<Grounded>()
			.Stream();
	private static readonly Stream<Velocity, MoveVelocity> toApplyMoveVel =
		world.Query<Velocity, MoveVelocity>()
			.Has<Grounded>()
			.Stream();
	private static void ApplyMoveVelocity(ref Velocity vel, ref MoveVelocity moveVel)
	{
		vel.Value += moveVel.Value;
	}	
}