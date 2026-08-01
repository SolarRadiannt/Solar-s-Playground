namespace SolFramework.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Managers;
using SolFramework.Scheduler;
using System.Linq;


public partial class HealthApply : Node, ISystem
{
	public int Priority => SPriority.Applying;
	public void Process(double _)
	{
		ApplyHeals();
		ApplyClamping();
		ApplyDamages();
	}

	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();

	private static readonly World world = Core.World;
	private static readonly Stream<Health, MaxHealth> toClampHealth =
		world.Stream<Health, MaxHealth>();
	private static void ApplyClamping()
	{
		toClampHealth.For(
			static (ref Health health, ref MaxHealth maxHealth) =>
			{
				health.Value = Math.Min(maxHealth.Value, health.Value);
			});
	}
	
	private static readonly Stream<DamageAmount, DamageTarget> toApplyDamage =
		world.Query<DamageAmount, DamageTarget>()
			.Has<DamageEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void ApplyDamages() =>
		toApplyDamage.For(static
		(in Entity eevent, ref DamageAmount amount, ref DamageTarget target) =>
		{
			if (!target.Value.Has<Health>() || amount.Value <= 0) return;
			target.Value.Ref<Health>().Value -= amount.Value;
		});
	
	
	private static readonly Stream<HealAmount, HealTarget> toApplyHealth =
		world.Query<HealAmount, HealTarget>()
			.Has<HealEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void ApplyHeals()
	{
		toApplyHealth.For(static
		(in Entity eevent, ref HealAmount amount, ref HealTarget target) =>
		{
			if (!target.Value.Has<Health>() || amount.Value <= 0) return;
			target.Value.Ref<Health>().Value += amount.Value;
		});
	}
}