using Godot;
using fennecs;

using SolTools.Components;


[GlobalClass]
public abstract partial class BaseTool2D : EcsArea2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Tool>();
	}
}