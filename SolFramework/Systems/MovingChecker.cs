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
	private static readonly World world = Core.World;
	public int Priority => SPriority.Transformation;
	public void Process(double delta)
	{
		toCheck.For(CheckMoving);
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<Velocity, ActualSpeed> toCheck =
		world.Stream<Velocity, ActualSpeed>();
	public static void CheckMoving(in Entity entity, ref Velocity vel, ref ActualSpeed speed)
	{
		float intendedSpeed = vel.Value.Length();
			
		bool moving = entity.Has<Moving>();
		bool isActuallyMoving = speed.Value > 0.1f;
		bool isTryingToMove = intendedSpeed > 0.1f;
		
		if (!moving && isActuallyMoving)
			entity.Add<Moving>();
		else if (moving && !isActuallyMoving)
			entity.Remove<Moving>();
		
		if (isTryingToMove && !isActuallyMoving)
			if (!entity.Has<MovingBlocked>())
				entity.Add<MovingBlocked>();
		else
			if (entity.Has<MovingBlocked>())
				entity.Remove<MovingBlocked>();
	}
}
