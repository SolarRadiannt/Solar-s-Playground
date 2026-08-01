namespace Root;

using Godot;

using SolFramework.Scheduler;
using SolFramework.Tools;
using SolProjectiles;
using SolProjectiles.Managers;

public partial class MainGame : Node2D
{
	public static EcsCharBody2D Player;
	public static MainGame Instance;
	private void AddChildBatch(Node[] nodes)
	{
		foreach (Node node in nodes)
			AddChild(node);
	}
	public override void _Ready()
	{
		Instance = this;
		
		AddChildBatch(SolFramework.SystemRegistry.All);
		AddChildBatch(SystemRegistry.All);
		AddChildBatch(SolProjectiles.SystemRegistry.All);
		AddChildBatch(SolActions.SystemRegistry.All);
		
		ProjectileRegistry.RegisterAllInFolder("res://Main/Projectiles", true);
		SolInspector.Init();
	}
	public override void _PhysicsProcess(double delta)
	{
		Scheduler.PorcessAllPhysics(delta);
	}

	public override void _Process(double delta)
	{
		Scheduler.ProcessAll(delta);
	}
}
