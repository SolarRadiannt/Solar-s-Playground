using Godot;
using Root.Components;
using SolItems.Components;
using System;

public partial class Rifle : EcsArea2D
{
	protected override void OnEntityReady()
	{
		GD.Print("firearm is ready!");
		entity.Add<ItemType<Firearm>>()
			.Add<Item>()
			.Add<Pickupable>();
	}
}
