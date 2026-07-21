using Godot;
using SolFramework.Components;
using SolFramework.FootstepManager;
using SolFramework.MoveManager;
using System;

public partial class Character : ECSCharBody2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Player>();
		entity.Add<Grounded>();
		MoveManager.ApplyMovement(entity, 400);
		FootstepManager.ApplyFootstep(entity, 150);
		GD.Print("Character spawned!");
	}
}
