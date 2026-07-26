namespace SolFramework.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Managers;
using SolFramework.Scheduler;



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
	
	private static readonly Stream<DamageTarget, DamageAmount> toApplyDamage =
		world.Query<DamageTarget, DamageAmount>()
			.Has<DamageEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void ApplyDamages()
	{
		toApplyDamage.For(
			static (ref DamageTarget target, ref DamageAmount amount) =>
			{
				if (!target.Value.Has<Health>() || amount.Value <= 0) return;
				target.Value.Ref<Health>().Value -= amount.Value;
			});
	}
	
	private static readonly Stream<HealTarget, HealAmount> toApplyHealth =
		world.Query<HealTarget, HealAmount>()
			.Has<HealEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void ApplyHeals()
	{
		toApplyHealth.For(
			static (ref HealTarget target, ref HealAmount amount) =>
			{
				if (!target.Value.Has<Health>() || amount.Value <= 0) return;
				target.Value.Ref<Health>().Value += amount.Value;
			});
	}
}