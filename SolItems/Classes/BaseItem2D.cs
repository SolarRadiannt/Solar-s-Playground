using Godot;
using fennecs;

using SolItems.Components;
using SolItems.Managers;


[GlobalClass]
public abstract partial class BaseItem2D : EcsArea2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Item>();
	}
}