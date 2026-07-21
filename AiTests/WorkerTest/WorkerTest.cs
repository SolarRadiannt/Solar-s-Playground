using Godot;
using SolFramework.Components;
using SolFramework.MoveManager;
using SolFramework.FootstepManager;
using FluidHTN;

public partial class WorkerTest : ECSCharBody2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Player>();
		entity.Add<Grounded>();
		MoveManager.ApplyMovement(entity, 400);
		FootstepManager.ApplyFootstep(entity, 150);
		
		
	}

	public override void _Ready()
	{
		
		base._Ready();
	}

}
