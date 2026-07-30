using Godot;
using SolFramework.Components;
using SolFramework.Managers;
using SolFramework.Tools;

using SolProjectiles;
using SolProjectiles.Managers;

public partial class CharacterPlayer : EcsCharBody2D
{
	public TickTimer firerate = new(1f, true);
	protected override void OnEntityReady()
	{
		entity.Add<Player>();
		entity.Add<Grounded>();
		MoveManager.ApplyMovement(entity, 400);
		HealthManager.ApplyHealth(entity, 100f);
		FootstepManager.ApplyFootstep(entity, 2f);
		
		GD.Print("Character spawned!");
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();	
		if (firerate.Tick(delta).JustFinished())
		{
			ProjectileManager.Shoot(ProjectileTypes.Rifle, GlobalPosition, Vector2.Left, entity, null, CollisionMask);
		}
	}

}
