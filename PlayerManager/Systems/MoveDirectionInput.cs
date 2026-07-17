namespace PlayerManager.Systems;
using fennecs;
using Godot;

using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Core;
using SolFramework.MoveManager;

public partial class MoveDirectionInput : Node, ISystem
{
	public int Priority => SPriority.Action - 1;
	public void Process(double _)
	{
		Control();
	}

	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready()
	{
		Init();
	}
	private static readonly World world = Core.World;
	private static readonly Stream<MoveDirection> controllable = world.Query<MoveDirection>()
		.Has<Player>()
		.Stream();
	private static void Control()
	{
		controllable.For(
			static (ref MoveDirection moveDir) =>
			{
				var dir = Vector2.Zero;
				if (Input.IsActionPressed("move_up"))
					dir += Vector2.Up;
				if (Input.IsActionPressed("move_down"))
					dir += Vector2.Down;
				if (Input.IsActionPressed("move_left"))
					dir += Vector2.Left;
				if (Input.IsActionPressed("move_right"))
					dir += Vector2.Right;
				
				GD.Print("input processed", dir);
				moveDir.Value = dir;
			});
	}
};
