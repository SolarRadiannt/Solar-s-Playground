namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework.Core;
using SolFramework.Scheduler;
using SolFramework.MoveManager;


public partial class Movement : Node, ISystem
{
	public int Priority => SPriority.Action;
	public void Process(double delta)
	{
		HandleMoveTo();
		ApplyMoveVelocity();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	
	public override void _Ready() => Init();

	private static readonly World world = Core.World;
	private static readonly Stream<MoveDirection, MoveSpeed, MoveVelocity> toApplyVel =
		world.Stream<MoveDirection, MoveSpeed, MoveVelocity>();
	private static void ApplyMoveVelocity()
	{
		toApplyVel.For(
			static (ref MoveDirection moveDir, ref MoveSpeed speed, ref MoveVelocity moveVel) =>
			{
				
				moveVel.Value = moveDir.Value.Normalized() * speed.Value;
			});
	}
	
	private static readonly Stream<ECSCharBody2D, MoveDirection, MoveGoal> toMoveTo =
		world.Stream<ECSCharBody2D, MoveDirection, MoveGoal>();
	private static void HandleMoveTo()
	{
		toMoveTo.For(
			static (in Entity entity, ref ECSCharBody2D body, ref MoveDirection moveDir, ref MoveGoal moveGoal) =>
			{
				var origin = body.GlobalPosition;
				var goal = moveGoal.Value;
				
				var resultant = origin - goal;
				
				if (resultant.Length() <= MoveManager.GetMoveToReach(entity))
					moveDir.Value = Vector2.Zero;
				else
					moveDir.Value = resultant.Normalized();
			});
	}
}