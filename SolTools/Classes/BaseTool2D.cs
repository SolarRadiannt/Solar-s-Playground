using Godot;
using fennecs;

using SolTools.Components;
using SolTools.Managers;


[GlobalClass]
public abstract partial class BaseTool2D : EcsArea2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Tool>();
	}

	public override void _Ready()
	{
		base._Ready();
		this.SetParent(ToolsManager.DroppedToolsContainer);
	}
}