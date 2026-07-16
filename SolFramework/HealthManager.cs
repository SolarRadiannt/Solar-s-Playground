namespace SolFramework.HealthManager
{
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
	
	
	
	public record struct DamageData(float Amount, Entity Target, Entity? Source = null!);
	public record struct HealData(float Amount, Entity Target, Entity? Source = null!);
	
	public static class HealthManager
	{
		static Entity Damage(DamageData data)
		{
			var entity = EEvent.Spawn()
				.Add(new DamageAmount(data.Amount))
				.Add(new DamageTarget(data.Target))
				.Add<DamageEvent>();
			
			// Only add Source if it has a value
			if (data.Source.HasValue)
			entity.Add(new DamageSource(data.Source.Value));

			return entity;
		}
		static Entity Heal(HealData data)
		{
			var entity = EEvent.Spawn()
				.Add(new HealAmount(data.Amount))
				.Add(new HealTarget(data.Target))
				.Add<HealEvent>();

			if (data.Source.HasValue)
			entity.Add(new HealSource(data.Source.Value));

			return entity;
		}
	}
}