using Godot;
using SolFramework.Scheduler;

public partial class MainNode : Node
{
	// Called when the node enters the scene tree for the first time.
	private void AddChildBatch(Node[] nodes)
	{
		foreach (Node node in nodes)
			AddChild(node);
	}
	
	public override void _Ready()
	{
		AddChildBatch(SolFramework.Systems.SystemRegistry.GetAll());
		AddChildBatch(PlayerManager.Systems.SystemRegistry.GetAll());
		AddChildBatch(Main.Systems.SystemRegistry.GetAll());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Scheduler.ProcessAll(delta);
	}
}
