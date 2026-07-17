using Godot;
using SolFramework.Components;
using SolFramework.MoveManager;
using System;

public partial class Character : ECSCharBody2D
{
	protected override void OnEntityReady()
	{
		entity
			.Add(new Velocity(Vector2.Zero))
			.Add(new MoveDirection(Vector2.Zero))
			.Add(new MoveVelocity(Vector2.Zero))
			.Add(new MoveSpeed(400))
			.Add<Player>();
		
		GD.Print("Character spawned!");
	}
}
