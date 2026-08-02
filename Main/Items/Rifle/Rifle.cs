using Godot;
using Root.Components;
using System;

public partial class Rifle : BaseItem2D
{
	protected override void OnEntityReady()
	{
		base.OnEntityReady();
		GD.Print("firearm is ready!");
		entity.Add<Firearm>();
	}
}
