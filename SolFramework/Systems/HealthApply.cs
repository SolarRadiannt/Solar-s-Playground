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
	
	private static readonly Stream<DamageEvent> toApplyDamage =
		world.Query<DamageEvent>()
			.Has<Damage>(Match.Entity)
			.Not<EventCancelled>()
			.Stream();
	private static void ApplyDamages()
	{
		toApplyDamage.For(static
		(in Entity eevent, ref DamageEvent _) =>
		{
			world.Query<Health>()
				.Has<Damage>(eevent)
				.Stream()
			.For(eevent, static
			(Entity ev, in Entity target, ref Health health) =>
			{
				float damage = target.Ref<Damage>(ev).Value;
				if (damage <= 0) return;

				health.Value -= damage;
			});
		});
	}
	
	private static readonly Stream<HealEvent> toApplyHealth =
		world.Query<HealEvent>()
			.Has<Heal>(Match.Entity)
			.Not<EventCancelled>()
			.Stream();
	private static void ApplyHeals()
	{
		toApplyHealth.For(static
		(in Entity eevent, ref HealEvent _) =>
		{
			world.Query<Health>()
				.Has<Heal>(eevent)
				.Stream()
			.For(eevent, static
			(Entity ev, in Entity target, ref Health health) =>
			{
				float heal = target.Ref<Heal>(ev).Value;
				if (heal <= 0) return;

				health.Value += heal;
			});
		});
	}
}