namespace SolFramework.Managers;

using fennecs;
using SolFramework;
using SolFramework.Tools;


[InspectorColor(InspectColor.Forest)] public record struct MaxHealth(float Value);
[InspectorColor(InspectColor.Forest)] public record struct Health(float Value);

public record struct DamageAmount(float Value);
public record struct DamageSource(Entity Value);
public record struct DamageTarget(Entity Value);
public record struct DamageWith(Entity Value);
public struct DamageEvent;


public record struct HealAmount(float Value);
public record struct HealSource(Entity Value);
public record struct HealTarget(Entity Value);
public record struct HealWith(Entity Value);
public struct HealEvent;


public static class HealthManager
{
	public static Entity ApplyHealth(Entity entity, float health) =>
		entity
			.Add(new Health(health))
			.Add(new MaxHealth(health));
	
	public static Entity Damage(float amount, Entity target, Entity? source = null!, Entity? with = null!)
	{
		var eevent = EEvent.Spawn()
			.Add<DamageEvent>()
			.Add(new DamageAmount(amount))
			.Add(new DamageTarget(target));
		
		if (source.HasValue)
			eevent.Add(new DamageSource(source.Value));
		
		if (with.HasValue)
			eevent.Add(new DamageWith(with.Value));
		
		return eevent;
	}
	
	public static Entity Heal(float amount, Entity target, Entity? source = null!, Entity? with = null!)
	{
		var eevent = EEvent.Spawn()
			.Add<HealEvent>()
			.Add(new HealAmount(amount))
			.Add(new HealTarget(target));

		if (source.HasValue)
			eevent.Add(new HealSource(source.Value));
		
		if (with.HasValue)
			eevent.Add(new HealWith(with.Value));
		
		return eevent;
	}
}