using Godot;
using fennecs;

using SolFramework.Managers;

using SolProjectiles.Components;

[GlobalClass]
public abstract partial class BaseProjectile : EcsNode2D
{
	protected abstract float Damage { get; }
	protected abstract float MaxDistance { get; }
	protected abstract float Speed {get; }
	
	public Vector2 Direction;
	public Entity Source;
	
	protected override void OnEntityReady()
	{
		entity
			.Add<Projectile>()
			.Add<LookAtMoveDir>()
			.Add(new ProjectileSource(Source))
			.Add(new ProjectileDamage(Damage))
			.Add(new ProjectileOrigin(GlobalPosition))
			.Add(new ProjectileMaxDistance(MaxDistance));
		
		MoveManager.ApplyMovement(entity, Speed);
		MoveManager.SetMoveDirection(entity, Direction);
	}
}
