namespace SolFramework.Managers;

using fennecs;
using SolFramework;
using SolFramework.Tools;


[InspectorColor(InspectColor.Forest)] public record struct MaxHealth(float Value);
[InspectorColor(InspectColor.Forest)] public record struct Health(float Value);

public record struct DamageWith;
public record struct HealWith;

public record struct DamageSource;
public record struct Damage(float Value);
public struct DamageEvent;

public record struct HealSource;
public record struct Heal(float Value);
public struct HealEvent;

public static class HealthManager
{
	public static Entity ApplyHealth(Entity entity, float health) =>
		entity
			.Add(new Health(health))
			.Add(new MaxHealth(health));
	
	public static Entity Damage(float amount, Entity[] targets, Entity[] sources = null!, Entity[] withs = null!)
	{
		var eevent = EEvent.Spawn()
			.Add<DamageEvent>();
		
		
		foreach (var e in targets)
			eevent.Add(new Damage(amount), e);

		if (sources != null)
			foreach (var e in sources)
				eevent.Add<DamageSource>(e);
		
		if (withs != null)
			foreach (var e in withs)
				eevent.Add<DamageWith>(e);

		return eevent;
	}

	public static Entity Damage(float amount, Entity target, Entity? source, Entity? with) =>
		Damage(amount, [target], source.HasValue ? [source.Value] : null, with.HasValue ? [with.Value] : null);

	public static Entity Damage(float amount, Entity[] targets, Entity? source, Entity? with) =>
		Damage(amount, targets, source.HasValue ? [source.Value] : null, with.HasValue ? [with.Value] : null);

	public static Entity Heal(float amount, Entity[] targets, Entity[] sources = null!, Entity[] withs = null!)
	{
		var eevent = EEvent.Spawn()
			.Add<HealEvent>();

		foreach (var e in targets)
			eevent.Add(new Heal(amount), e);
		
		if (sources != null)
			foreach (var e in sources)
				eevent.Add<HealSource>(e);

		if (withs != null)
			foreach (var e in withs)
				eevent.Add<HealWith>(e);
		
		return eevent;
	}

	public static Entity Heal(float amount, Entity target, Entity? source, Entity? with) =>
		Heal(amount, [target], source.HasValue ? [source.Value] : null, with.HasValue ? [with.Value] : null);
	
	public static Entity Heal(float amount, Entity[] targets, Entity? source, Entity? with) =>
		Heal(amount, targets, source.HasValue ? [source.Value] : null, with.HasValue ? [with.Value] : null);
}