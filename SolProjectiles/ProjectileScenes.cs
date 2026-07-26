namespace SolProjectiles;
using Godot;
using System;

public partial class ProjectileScenes : Node
{
	[Export] public PackedScene BasicProjectile;
	
	public static ProjectileScenes Instance;
	public override void _Ready()
	{
		Instance = this;
	}
}
