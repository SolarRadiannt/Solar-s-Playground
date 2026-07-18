using Godot;
using SolFramework.Components;
using SolFramework.MoveManager;
using System;

public partial class Character : ECSCharBody2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Player>();
		MoveManager.ApplyMovement(entity, 400);
		GD.Print("Character spawned!");
	}
}
