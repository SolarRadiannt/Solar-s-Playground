using Godot;
using fennecs;

using SolFramework.Managers;

using SolProjectiles.Components;

[GlobalClass]
public partial class EcsProjectile2D : EcsNode2D
{
	protected override void OnEntityReady()
	{
		entity
			.Add<Projectile>()
			.Add<LookAtMoveDir>();
	}
}
