namespace SolFramework.Systems;
using Godot;
using fennecs;
using SolFramework.Core;
using SolFramework.HealthManager;
using SolFramework.Scheduler;
using System.ComponentModel.DataAnnotations;
using SolFramework.EEvents;
using System;


public partial class HealthApply : Node, ISystem
{
	public int Priority => SPriority.Applying;
	public void Process(double _)
	{
		ApplyHeals();
		ClampHealths();
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
	private static void ClampHealths()
	{
		toClampHealth.For(
			static (ref Health health, ref MaxHealth maxHealth) =>
			{
				health.Value = Math.Min(maxHealth.Value, health.Value);
			});
	}
	
	private static readonly Stream<DamageTarget, DamageAmount> toApplyDamage =
		world.Query<DamageTarget, DamageAmount>().Not<EventCancelled>().Stream();
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
		world.Query<HealTarget, HealAmount>().Not<EventCancelled>().Stream();
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