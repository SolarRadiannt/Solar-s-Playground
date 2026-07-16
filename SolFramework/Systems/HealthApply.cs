namespace SolFramework.Systems;
using Godot;
using fennecs;
using SolFramework.Core;
using SolFramework.HealthManager;
using SolFramework.Scheduler;
using System.ComponentModel.DataAnnotations;

public partial class HealthApply : Node, ISystem
{
	public int Priority => SPriority.Applying;
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public void Process(double _)
	{
		_applyHeal();
		_applyDamage();
	}
	
	private static World world = Core.World;
	private static Stream<DamageTarget, DamageAmount> queryApplyDamage =
		world.Stream<DamageTarget, DamageAmount>();
	private static Stream<HealTarget, HealAmount> queryApplyHeal =
		world.Stream<HealTarget, HealAmount>();
	
	private static void _applyDamage()
	{
		queryApplyDamage.For(
			static (ref DamageTarget t, ref DamageAmount a) =>
			{
				var target = t.Value;
				var amount = a.Value;
				
				if (target.Has<Health>())
					target.Ref<Health>().Value -= amount;
				
			});
	}
	private static void _applyHeal()
	{
		queryApplyHeal.For(
			static (ref HealTarget t, ref HealAmount a) =>
			{
				var target = t.Value;
				var amount = a.Value;
				
				if (!target.Has<Health>()) return;
				
				ref var health = ref target.Ref<Health>(); 
				
				if (target.Has<MaxHealth>())
					health.Value = Mathf.Min(health.Value + amount, target.Ref<MaxHealth>().Value);
				else
					health.Value += amount;
			});
	}
	
	public override void _Ready()
	{
		Init();
	}
}