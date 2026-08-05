namespace SolProjectiles.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;

using SolProjectiles.Components;

public partial class FarDestroyer : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Interception;
	public void Process(double delta)
	{
		projectiles.For(TooFarCheck);
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	private static Stream<EcsNode2D, ProjectileOrigin, ProjectileMaxDistance> projectiles = 
		world.Query<EcsNode2D, ProjectileOrigin, ProjectileMaxDistance>()
		.Has<Projectile>()
		.Not<Destroy>()
		.Stream();
	private static void TooFarCheck(
		in Entity entity,
		ref EcsNode2D projectile,
		ref ProjectileOrigin origin,
		ref ProjectileMaxDistance maxDist
	) {
		float distance = (origin.Value - projectile.GlobalPosition).Length();

		if (!entity.Has<ProjectileCurrentDistance>())
			entity.Add(new ProjectileCurrentDistance(distance));
		else
			entity.Ref<ProjectileCurrentDistance>().Value = distance;

		if (distance >= maxDist.Value)
			entity.Add<Destroy>();
	}
}
