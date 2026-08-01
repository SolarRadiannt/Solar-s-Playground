using Godot;
using Root.Components;
using System;

public partial class Rifle : BaseTool2D
{
	protected override void OnEntityReady()
	{
		base.OnEntityReady();
		entity.Add<Firearm>();
	}
}
