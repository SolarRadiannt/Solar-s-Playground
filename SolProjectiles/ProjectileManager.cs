namespace SolProjectiles.Managers;

using Root;
using fennecs;
using Godot;

using SolFramework;
using SolProjectiles.Components;


// merge this mater into SolFramework
public static class ProjectileManager
{
	private static Node ProjectileContainer => MainGame.Instance; // turn this into SolFramework.Config.ProjectileContainer
	public static BaseProjectile Shoot(PackedScene projectileScene, Vector2 origin, Vector2 direction, Entity source)
	{
		var projectile = projectileScene.Instantiate<BaseProjectile>();
		projectile.GlobalPosition = origin;
		projectile.Direction = direction;
		projectile.Source = source;
		projectile.LookAt(origin + direction);
		
		ProjectileContainer.AddChild(projectile);
		
		EEvent.Spawn()
			.Add(new ShootOrigin(origin))
			.Add(new ShootProjectile(projectile))
			.Add(new ShootSource(source))
			.Add<ShootEvent>();
		
		
		return projectile;
	}
}