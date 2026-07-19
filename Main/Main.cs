namespace Root;

using Godot;
using SolFramework.Scheduler;

public partial class Main : Node2D
{
	public static Main Instance;
	private void AddChildBatch(Node[] nodes)
	{
		foreach (Node node in nodes)
			AddChild(node);
	}
	private static bool init = false;
	public override void _EnterTree()
	{
		if (init) return;
		init = true;
		Instance = this;
		AddChildBatch(SolFramework.Systems.SystemRegistry.GetAll());
		AddChildBatch(PlayerManager.Systems.SystemRegistry.GetAll());
		AddChildBatch(Root.Systems.SystemRegistry.GetAll());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Scheduler.ProcessAll(delta);
	}
}
