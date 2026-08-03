namespace SolActions.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.Managers;
using GodotUtilities;
using SolFramework.Components;

public partial class HandleWanderer : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	public void Process(double delta)
	{
		CooldownTicker(delta);
		WanderGoalSetter();
		WanderGoalChecker();
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<WanderCooldown> tickCooldown =
		world.Query<WanderCooldown>()
			.Not<Wandering>()
			.Stream();
	private static void CooldownTicker(double delta) =>
		tickCooldown.For((float)delta, static
		(float dt, in Entity entity, ref WanderCooldown cooldown) =>
		{
			cooldown.Value.Tick(dt);
		});
	
	private static readonly Stream<Node2DHandle> toSetWanderGoal = 
		world.Query<Node2DHandle>()
		.Has<Wandering>()
		.Not<WanderGoal>()
		.Stream();
	private static void WanderGoalSetter() =>
	toSetWanderGoal.For(static
	(in Entity entity, ref Node2DHandle handle) =>
	{
		float range = WanderAction.WANDER_RADIUS;
		var origin = handle.Value.GlobalPosition;
		var goal = origin + SolRand.Vec2Radius(range);

		entity.Add(new WanderGoal(goal));
		MoveManager.MoveTo(entity, goal);
	});

	private static readonly Stream<Node2DHandle, WanderGoal> toCheckWanderReached=
		world.Query<Node2DHandle, WanderGoal>()
		.Has<Wandering>()
		.Stream();
	private static void WanderGoalChecker() =>
	toCheckWanderReached.For(static
	(in Entity entity, ref Node2DHandle handle, ref WanderGoal goal) =>
	{
		float reachDist = MoveManager.GetReachDist(entity);
		float dist = handle.Value.GlobalPosition.DistanceTo(goal.Value);
		if (dist <= reachDist)
			entity.Remove<WanderGoal>();
	});

}
