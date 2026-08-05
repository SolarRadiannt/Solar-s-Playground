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

public partial class ResolveHit : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying;
	public void Process(double delta)
	{
		projectiles.For((float)delta, MoveAndCheckHit);
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
	private static Stream<EcsNode2D, Velocity, ProjectileDamage, ProjectileOrigin> projectiles =
		world.Query<EcsNode2D, Velocity, ProjectileDamage, ProjectileOrigin>()
		.Has<Projectile>()
		.Not<Destroy>()
		.Stream();
	private static void MoveAndCheckHit(
		float delta,
		in Entity entity,
		ref EcsNode2D projectile,
		ref Velocity vel,
		ref ProjectileDamage damage,
		ref ProjectileOrigin origin
	) {
		var pos = projectile.GlobalPosition;
		var nextPos = pos + (vel.Value * delta);

		var resultant = nextPos - pos;
		var dir = resultant.Normalized();
		float dist = resultant.Length();
		
		uint mask = entity.TryRead<ProjectileCollisionMask>(out var maskComp) 
			? maskComp.Value
			: uint.MaxValue;
		
		if (!PhysicsQuery2D.Raycast(origin: pos, direction: dir, dist, out var result, mask))
		{
			projectile.GlobalPosition = nextPos;
			return;
		}
		
		var hitEvent = EEvent.Spawn()
			.Add<HitEvent>()
			.Add(new HitDataNormal(result.Normal))
			.Add(new HitDataPosition(result.Position))
			.Add(new HitDataObject(result.Collider))
			.Add(new HitDataRid(result.ColliderRid))
			.Add(new HitDataOrigin(origin.Value))
			.Add(new HitDataDistance(entity.Ref<ProjectileCurrentDistance>().Value))
			.Add(new HitDataDamage(damage.Value));
		
		if (entity.Has<ProjectileWeapon>())
			hitEvent.Add(new HitDataWeapon(entity.Ref<ProjectileWeapon>().Value));

		if (entity.Has<ProjectileSource>())
			hitEvent.Add(new HitDataSource(entity.Ref<ProjectileSource>().Value));

		entity
			.Add(new ProjectileHitEvent(hitEvent))
			.Add<Destroy>();
	}
}
