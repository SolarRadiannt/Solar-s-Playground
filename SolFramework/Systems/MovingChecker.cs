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
	private static readonly World world = Core.World;
	public int Priority => SPriority.Init;
	public void Process(double delta)
	{
		toCheck.For(
			static(in Entity entity, ref Velocity vel) =>
			{
				float speed = vel.Value.Length();
				bool moving = entity.Has<Moving>();
				
				if (!moving && speed > 0.01)
					entity.Add<Moving>();
				else if (moving && speed < 0.01)
					entity.Remove<Moving>();
			});
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	public static readonly Stream<Velocity> toCheck = world.Stream<Velocity>();
}
