namespace SolProjectiles.Managers;

using Root;
using fennecs;
using Godot;

using SolFramework;
using SolProjectiles.Components;

public static class ProjectileManager
{
	// public static void Init()
	// {
	// 	ProjectileRegistry.RegisterAllInFolder("res://SolProjectiles/Projectiles", true);
	// }
	
	public static Entity Shoot(StringName projectileType, Vector2 origin, Vector2 direction, Entity? source, Entity? weapon, uint? collisionMask)
	{
		var e = EEvent.Spawn()
			.Add(new ShootOrigin(origin))
			.Add(new ShootProjectileType(projectileType))
			.Add(new ShootDirection(direction))
			.Add<ShootEvent>();
		
		if (source.HasValue)
			e.Add(new ShootSource(source.Value));
			
		if (weapon.HasValue)
			e.Add(new ShootWeapon(weapon.Value));
		
		if (collisionMask.HasValue)
			e.Add(new ShootCollisionMask(collisionMask.Value));

		return e;
	}

	public static Entity Shoot(StringName projectileType, Vector2 origin, Vector2 direction, uint collisionMask) =>
		Shoot(projectileType, origin, direction, null, null, collisionMask);
	
}