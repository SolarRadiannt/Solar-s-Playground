using Godot;
using Root;
using SolFramework.Components;
using SolFramework.Managers;
using SolFramework.Tools;

using SolProjectiles;
using SolProjectiles.Managers;

public partial class CharacterPlayer : EcsCharBody2D
{
	public TickTimer firerate = new(0.15f, true);
	protected override void OnEntityReady()
	{
		entity.Add<Player>();
		entity.Add<Grounded>();
		MoveManager.ApplyMovement(entity, 400);
		HealthManager.ApplyHealth(entity, 100f);
		FootstepManager.ApplyFootstep(entity, 2.5f);
		
		GD.Print("Character spawned!");
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
		firerate.Tick(delta);
		if (Input.IsActionPressed("shoot") && firerate.JustFinished())
		{
			var dir = (MainGame.Instance.GetGlobalMousePosition() - GlobalPosition).Normalized();

			ProjectileManager.Shoot(ProjectileTypes.Rifle, GlobalPosition, dir, entity, null, CollisionMask);
		}
	}

}
