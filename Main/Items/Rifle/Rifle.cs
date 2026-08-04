using Godot;
using Root.Components;
using SolItems.Components;
using System;

public partial class Rifle : EcsArea2D
{
	protected override void OnEntityReady()
	{
		GD.Print("firearm is ready!");
		entity
			.Add<Item>()
			.Add<Pickupable>();
			// .Add<ItemType<Firearm>>()
			// .Add<FirearmType<Root.Components.Rifle>>();
	}
}
