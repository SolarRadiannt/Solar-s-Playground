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
		_applyHeal();
		_clampHealth();
		_applyDamage();
	}
	
	private static World world = Core.World;
	private static Stream<Health, MaxHealth> toClampHealth =
		world.Stream<Health, MaxHealth>();
	private static void _clampHealth()
	{
		toClampHealth.For(
			static (ref Health health, ref MaxHealth maxHealth) =>
			{
				health.Value = Math.Min(maxHealth.Value, health.Value);
			});
	}
	
	private static Stream<DamageTarget, DamageAmount> toApplyDamage =
		world.Query<DamageTarget, DamageAmount>().Not<EventCancelled>().Stream();
	private static void _applyDamage()
	{
		toApplyDamage.For(
			static (ref DamageTarget target, ref DamageAmount amount) =>
			{
				if (!target.Value.Has<Health>() || amount.Value <= 0) return;
				target.Value.Ref<Health>().Value -= amount.Value;
			});
	}
	
	private static Stream<HealTarget, HealAmount> toApplyHealth =
		world.Query<HealTarget, HealAmount>().Not<EventCancelled>().Stream();
	private static void _applyHeal()
	{
		toApplyHealth.For(
			static (ref HealTarget target, ref HealAmount amount) =>
			{
				if (!target.Value.Has<Health>() || amount.Value <= 0) return;
				target.Value.Ref<Health>().Value += amount.Value;
			});
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready()
	{
		Init();
	}
}