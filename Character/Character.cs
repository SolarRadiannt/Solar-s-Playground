using Godot;
using SolFramework.Components;
using SolFramework.Managers;

public partial class Character : EcsCharBody2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Player>();
		entity.Add<Grounded>();
		MoveManager.ApplyMovement(entity, 400);
		FootstepManager.ApplyFootstep(entity, 3f);
		GD.Print("Character spawned!");
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

}
