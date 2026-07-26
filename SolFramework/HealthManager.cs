namespace SolFramework.Managers;

using fennecs;
using SolFramework;

public record struct MaxHealth(float Value);
public record struct Health(float Value);

public record struct DamageWith(Entity Value);
public record struct HealWith(Entity Value);

public record struct DamageSource(Entity Value);
public record struct DamageTarget(Entity Value);
public record struct DamageAmount(float Value);
public struct DamageEvent;

public record struct HealSource(Entity Value);
public record struct HealTarget(Entity Value);
public record struct HealAmount(float Value);
public struct HealEvent;

public static class HealthManager
{
	public static Entity ApplyHealth(Entity entity, float health) =>
		entity
			.Add(new Health(health))
			.Add(new MaxHealth(health));
	
	public static Entity Damage(float amount, Entity target, Entity? source = null!)
	{
		var entity = EEvent.Spawn()
			.Add(new DamageAmount(amount))
			.Add(new DamageTarget(target))
			.Add<DamageEvent>();
		
		if (source.HasValue)
			entity.Add(new DamageSource(source.Value));

		return entity;
	}
	public static Entity Heal(float amount, Entity target, Entity? source = null!)
	{
		var entity = EEvent.Spawn()
			.Add(new HealAmount(amount))
			.Add(new HealTarget(target))
			.Add<HealEvent>();

		if (source.HasValue)
			entity.Add(new HealSource(source.Value));

		return entity;
	}
}