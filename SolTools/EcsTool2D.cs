using Godot;
using fennecs;

using SolTools.Components;


[GlobalClass]
public partial class EcsTool2D : EcsArea2D
{
	protected override void OnEntityReady()
	{
		entity
			.Add<Tool>();
	}
}