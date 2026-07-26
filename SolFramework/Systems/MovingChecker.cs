namespace SolFramework.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;

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
		GD.Print("moving checker initialized");
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<EcsCharBody2D, Velocity, LastPosition> toCheck =
		world.Stream<EcsCharBody2D, Velocity, LastPosition>();
	public static void CheckMoving() =>
		toCheck.For(static
		(in Entity entity, ref EcsCharBody2D body, ref Velocity vel, ref LastPosition lastPos) =>
		{
			var current = body.GlobalPosition;
			var last = lastPos.Value;
			var delta = current - last;

			float speed = delta.Length();
			float intendedSpeed = vel.Value.Length();
			
			bool moving = entity.Has<Moving>();
			bool isActuallyMoving = speed > 0.1f;
			bool isTryingToMove = intendedSpeed > 0.1f;
			
			if (!moving && isActuallyMoving)
				entity.Add<Moving>();
			else if (moving && !isActuallyMoving)
				entity.Remove<Moving>();
			
			if (isTryingToMove && !isActuallyMoving)
			{
				if (!entity.Has<MovingBlocked>())
					entity.Add<MovingBlocked>();
			}
			else
			{
				if (entity.Has<MovingBlocked>())
					entity.Remove<MovingBlocked>();
			}
		});
	
	public static readonly Stream<EcsCharBody2D> toUpdate = world.Stream<EcsCharBody2D>();
	public static void UpdateLastPosition() =>
		toUpdate.For(static
		(in Entity entity, ref EcsCharBody2D body) =>
		{
			if (entity.Has<LastPosition>())
				entity.Ref<LastPosition>().Value = body.GlobalPosition;
			else
				entity.Add(new LastPosition(body.GlobalPosition));
		});
	
}
