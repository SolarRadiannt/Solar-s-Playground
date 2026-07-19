namespace SolFramework.Systems;

using Godot;
using System;
using fennecs;

using SolFramework.Components;
using SolFramework.Core;
using SolFramework.Scheduler;
using SolFramework.MoveManager;

public partial class MovingChecker : Node, ISystem
{
	private record struct LastPosition(Vector2 Value);
	private static readonly World world = Core.World;
	public int Priority => SPriority.Init;
	public void Process(double delta)
	{
		CheckMoving();
		UpdateLastPosition();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<ECSCharBody2D, LastPosition> toCheck =
		world.Stream<ECSCharBody2D, LastPosition>();
	public static void CheckMoving() =>
		toCheck.For(
		static (in Entity entity, ref ECSCharBody2D body, ref LastPosition lastPos) =>
		{
			var current = body.GlobalPosition;
			var last = lastPos.Value;
			
			var delta = current - last;
			
			float speed = delta.Length();
			bool moving = entity.Has<Moving>();
				
			if (!moving && speed > 0.01)
				entity.Add<Moving>();
			else if (moving && speed < 0.01)
				entity.Remove<Moving>();
		});
	
	public static readonly Stream<ECSCharBody2D> toUpdate = world.Stream<ECSCharBody2D>();
	public static void UpdateLastPosition() =>
		toUpdate.For(static (in Entity entity, ref ECSCharBody2D body) =>
		{
			if (entity.Has<LastPosition>())
			{
				entity.Ref<LastPosition>().Value = body.GlobalPosition;
			}
			else
			{
				entity.Add(new LastPosition(body.GlobalPosition));
			}
		});
	
}
