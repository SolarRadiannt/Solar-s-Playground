namespace SolActions.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.Managers;
using GodotUtilities;


public partial class HandleWanderer : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	public void Process(double delta)
	{
		CooldownTicker(delta);
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
	
	private static readonly Stream<EcsCharBody2D> toSetWanderGoal = 
		world.Query<EcsCharBody2D>()
		.Has<Wandering>()
		.Not<WanderGoal>()
		.Stream();
	private static void WanderGoalSetter() =>
		toSetWanderGoal.For(static
		(in Entity entity, ref EcsCharBody2D body) =>
		{
			float range = WanderAction.WANDER_RADIUS;
			var origin = body.GlobalPosition;
			var goal = origin + SolRand.Vec2Radius(range);
		});
}
