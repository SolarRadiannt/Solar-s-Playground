namespace SolProjectiles.Managers;

using Root;
using fennecs;
using Godot;

using SolFramework;
using SolProjectiles.Components;

public static class ProjectileManager
{
	public static Entity Shoot(StringName projectileType, Vector2 origin, Vector2 direction, Entity? source, Entity? weapon)
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
		
		return e;
	}
}