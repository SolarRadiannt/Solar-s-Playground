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
		TooFarCheck();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	private static Stream<EcsCharBody2D, ProjectileOrigin, ProjectileMaxDistance> projectiles = 
		world.Query<EcsCharBody2D, ProjectileOrigin, ProjectileMaxDistance>()
		.Has<Projectile>()
		.Not<Destroy>()
		.Stream();
	private static void TooFarCheck() =>
		projectiles.For(
		static
		(
			in Entity entity,
			ref EcsCharBody2D body,
			ref ProjectileOrigin origin,
			ref ProjectileMaxDistance maxDist
		) => {
			float distance = (origin.Value - body.GlobalPosition).Length();
			if (distance >= maxDist.Value)
				entity.Add<Destroy>();
		});
}
