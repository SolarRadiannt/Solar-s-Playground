namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework.Core;
using SolFramework.Scheduler;
using SolFramework.MoveManager;


public partial class Movement : Node, ISystem
{
	private static World world = Core.World;
	private static Stream<MoveDirection, MoveSpeed, MoveVelocity> query_apply_movevel =
		world.Stream<MoveDirection, MoveSpeed, MoveVelocity>();
	private static void _SystemApplyMoveVel()
	{
		query_apply_movevel.For(
			static (ref MoveDirection moveDir, ref MoveSpeed speed, ref MoveVelocity moveVel) =>
			{
				moveVel.Value = moveDir.Value.Normalized() * speed.Value;
			});
	}
	
	private static Stream<ECSCharBody2D, MoveDirection, MoveGoal> query_moveto =
		world.Stream<ECSCharBody2D, MoveDirection, MoveGoal>();
	private static void _SystemMoveTo()
	{
		query_moveto.For(
			static (ref ECSCharBody2D body, ref MoveDirection moveDir, ref MoveGoal moveGoal) =>
			{
				var origin = body.Position;
				var goal = moveGoal.Value;
				
				var direction = (origin - goal).Normalized();
				moveDir.Value = direction;
			});
	}
	
	public int Priority => SPriority.Default;
	public void Process(double delta)
	{
		_SystemMoveTo();
		_SystemApplyMoveVel();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	
	public override void _Ready()
	{
		Init();
	}
}