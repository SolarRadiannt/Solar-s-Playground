namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.Managers;

using GodotUtilities;


public partial class LookToMoveDirection : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying;
	public void Process(double delta)
	{
		LookAtMoveDirection(delta);
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<Node2D, MoveDirection> toLookAtMoveDirection =
		world.Query<Node2D, MoveDirection>()
			.Has<LookAtMoveDir>()
			.Has<Moving>()
			.Stream();
	private static void LookAtMoveDirection(double delta) =>
		toLookAtMoveDirection.For((float)delta, static
		(float dt, in Entity entity, ref Node2D node, ref MoveDirection dir) =>
		{
			if (entity.Has<LookSpeed>())
				node.SmoothlyLookAt(node.GlobalPosition + dir.Value, entity.Ref<LookSpeed>().Value, dt);
			else
				node.LookAt(node.GlobalPosition + dir.Value);
		});
}
