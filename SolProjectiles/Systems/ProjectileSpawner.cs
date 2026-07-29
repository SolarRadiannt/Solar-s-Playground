namespace SolProjectiles.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;

using SolProjectiles.Components;
using Root;

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
	private static Stream<ShootProjectileType, ShootOrigin, ShootDirection, ShootSource> spawnEvents =
		world.Query<ShootProjectileType, ShootOrigin, ShootDirection, ShootSource>()
        .Not<EventCancelled>()
		.Has<ShootEvent>()
		.Stream();
	private static void ProjectileSpawning() =>
		spawnEvents.For(
		static(
			in Entity pe,
            ref ShootProjectileType type,
			ref ShootOrigin origin,
            ref ShootDirection direction,
			ref ShootSource source
		) => {
			if (!ProjectileRegistry.TryGetScene(type.Value, out var scene)) return;

			var projectile = scene.Instantiate<BaseProjectile>();
			projectile.GlobalPosition = origin.Value;
			projectile.Direction = direction.Value;
			projectile.Source = source.Value;
			projectile.LookAt(origin.Value + direction.Value);
			MainGame.Instance.AddChild(projectile);
		});
}
