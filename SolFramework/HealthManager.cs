namespace SolFramework.HealthManager;

using fennecs;
	using SolFramework.EEvents;
	
	public record struct MaxHealth(float Value);
	public record struct Health(float Value);
	
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
		static Entity Damage(float amount, Entity target, Entity? source = null!)
		{
			var entity = EEvent.Spawn()
				.Add(new DamageAmount(amount))
				.Add(new DamageTarget(target))
				.Add<DamageEvent>();
			
			// Only add Source if it has a value
			if (source.HasValue)
				entity.Add(new DamageSource(source.Value));

			return entity;
		}
		static Entity Heal(float amount, Entity target, Entity? source = null!)
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