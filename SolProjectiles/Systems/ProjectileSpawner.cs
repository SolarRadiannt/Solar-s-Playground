namespace SolProjectiles.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;

using SolProjectiles.Components;
using Root;
using SolFramework.Managers;
using Mapster;
using GodotUtilities;


public partial class ProjectileSpawner : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying - 100;
	public void Process(double delta)
	{
		ProjectileSpawning();
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
	private static Stream<ShootProjectileType, ShootOrigin, ShootDirection> spawnEvents =
		world.Query<ShootProjectileType, ShootOrigin, ShootDirection>()
        .Not<EventCancelled>()
		.Has<ShootEvent>()
		.Stream();
	private static void ProjectileSpawning() =>
		spawnEvents.For(
		static(
			in Entity reqEntity,
            ref ShootProjectileType type,
			ref ShootOrigin origin,
            ref ShootDirection direction
		) => {
			if (!ProjectileRegistry.TryGetData(type.Value, out var data))
				return;
			
			var projectile = data.Scene.Instantiate<EcsProjectile2D>();
			projectile.GlobalPosition = origin.Value;
			projectile.LookAtDir(direction.Value);
			projectile.Init();
			
			var entity = projectile.Entity;

			if (reqEntity.TryRead<ShootSource>(out var source))
				entity.Adapt(new ProjectileSource(source.Value));

			if (reqEntity.TryRead<ShootWeapon>(out var weapon))
				entity.Add(new ProjectileWeapon(weapon.Value));
			
			if (reqEntity.TryRead<ShootCollisionMask>(out var mask))
				entity.Add(new ProjectileCollisionMask(mask.Value));

			MoveManager.ApplyMovement(entity, data.Speed);
			MoveManager.SetMoveDirection(entity, direction.Value);
			MainGame.Instance.AddChild(projectile);
		});
}
