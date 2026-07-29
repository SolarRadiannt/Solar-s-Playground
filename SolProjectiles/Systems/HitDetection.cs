namespace SolProjectiles.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;

using SolProjectiles.Components;
using GodotUtilities;
using Root;

public partial class HitDetection : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying;
	public void Process(double delta)
	{
		ProcessProjectileHit(delta);
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
	private static Stream<EcsNode2D, Velocity, ProjectileDamage, ProjectileSource, ProjectileOrigin> projectiles =
		world.Query<EcsNode2D, Velocity, ProjectileDamage, ProjectileSource, ProjectileOrigin>()
		.Has<Projectile>()
		.Not<Destroy>()
		.Stream();
	private static void ProcessProjectileHit(double delta) =>
		projectiles.For(
		(float)delta,
		static(
			float delta,
			in Entity entity,
			ref EcsNode2D projectile,
			ref Velocity vel,
			ref ProjectileDamage damage,
			ref ProjectileSource source,
			ref ProjectileOrigin origin
		) => {
			var pos = projectile.GlobalPosition;
			var nextPos = pos * (vel.Value * delta);

			var resultant = pos - nextPos;
			var dir = resultant.Normalized();
			float dist = resultant.Length();
			
			if (PhysicsQuery2D.Raycast(origin: pos, direction: dir, dist, out var result))
			{
				var hitEvent = EEvent.Spawn()
					.Add<HitEvent>()
					.Add(new HitDataNormal(result.Normal))
					.Add(new HitDataPosition(result.Position))
					.Add(new HitDataObject(result.Collider))
					.Add(new HitDataRid(result.ColliderRid))
					.Add(new HitDataOrigin(origin.Value))
					.Add(new HitDataDistance(entity.Ref<ProjectileCurrentDistance>().Value));
				
				entity
					.Add(new ProjectileHitEvent(hitEvent))
					.Add<Destroy>();
			}
			else
				projectile.GlobalPosition = nextPos;
		});
}
