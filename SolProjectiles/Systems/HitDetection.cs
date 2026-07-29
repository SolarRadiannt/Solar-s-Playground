namespace SolProjectiles.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;

using SolProjectiles.Components;

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
	private static Stream<EcsCharBody2D, ProjectileDamage, ProjectileSource> projectiles =
		world.Query<EcsCharBody2D, ProjectileDamage, ProjectileSource>()
		.Has<Projectile>()
		.Not<Destroy>()
		.Stream();
	private static void ProcessProjectileHit(double delta) =>
		projectiles.For(
		(float)delta,
		static(
			float delta,
			in Entity entity,
			ref EcsCharBody2D body,
			ref ProjectileDamage damage,
			ref ProjectileSource source
		) => {
			var data = body.MoveAndCollide(body.Velocity * delta);
			if (data == null) return;
			
			if (data.GetCollider() is EcsCharBody2D otherBody)
			{
				if (otherBody.Entity.ToRaw() == source.Value.ToRaw()) return;
				GD.Print($"{Core.GetName(entity)} has hit and damaged {Core.GetName(otherBody.Entity)}");
				HealthManager.Damage(damage.Value, otherBody.Entity, source.Value)
					.Add(new DamageWith(entity)); // spawn transient event entity
			};
			
			entity.Add(new ProjectileHitData(data));
			entity.Add<Destroy>();
		});
}
