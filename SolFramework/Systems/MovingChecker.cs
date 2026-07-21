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
		GD.Print("moving checker initialized");
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<ECSCharBody2D, Velocity, LastPosition> toCheck =
		world.Stream<ECSCharBody2D, Velocity, LastPosition>();
	public static void CheckMoving() =>
		toCheck.For(static
		(in Entity entity, ref ECSCharBody2D body, ref Velocity vel, ref LastPosition lastPos) =>
		{
			var current = body.GlobalPosition;
			var last = lastPos.Value;
			var delta = current - last;
			float speed = delta.Length();
			bool moving = entity.Has<Moving>();

			// New: intended speed from velocity
			float intendedSpeed = vel.Value.Length();

			// Actual movement threshold
			bool isActuallyMoving = speed > 0.01f;
			// Trying to move threshold
			bool isTryingToMove = intendedSpeed > 0.01f;

			// Update Moving component based on actual movement
			if (!moving && isActuallyMoving)
				{entity.Add<Moving>(); GD.Print("Moving!");}
			else if (moving && !isActuallyMoving)
				{entity.Remove<Moving>(); GD.Print("Stopped!");};

			// Detect blocked state: trying to move but not actually moving
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
	
	public static readonly Stream<ECSCharBody2D> toUpdate = world.Stream<ECSCharBody2D>();
	public static void UpdateLastPosition() =>
		toUpdate.For(static
		(in Entity entity, ref ECSCharBody2D body) =>
		{
			if (entity.Has<LastPosition>())
				entity.Ref<LastPosition>().Value = body.GlobalPosition;
			else
				entity.Add(new LastPosition(body.GlobalPosition));
		});
	
}
