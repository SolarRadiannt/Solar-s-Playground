namespace SolItems.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;
using SolItems.Components;
using System.Linq;
using Root;
using SolItems.Managers;

public partial class PickupAndDrop : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying - 1;
	public void Process(double delta)
	{
		toPickup.For(HandlePickup);
		toPickup.For(ReadPickupRelationship);
		
		toDrop.For(HandleDrop);
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<PickupBy, PickupItem> toPickup =
		world.Query<PickupBy, PickupItem>()
			.Has<PickupEvent>()
			.Not<EventCancelled>()
			.Not<Visuals>()
			.Stream();
	private static void HandlePickup(ref PickupBy newOwner, ref PickupItem item)
	{
		OwnershipManager.SetOwner(newOwner.Value, item.Value);
			
		if (item.Value.TryRead<EcsArea2D>(out var itemHandle))
		{
			if (!newOwner.Value.TryRead<Node2D>(out var ownerHandle)) return;
			
			itemHandle.Visible = false;
			itemHandle.SetDeferred("disabled", true);
			itemHandle.SetParent(ownerHandle);
		}
	}
	
	private static void ReadPickupRelationship(ref PickupBy newOwner, ref PickupItem item)
	{
		GD.Print("reading system run");
		GD.Print($"Is {item.Value.GetName()} owned by {newOwner.Value.GetName()}: ",
			item.Value.Has<OwnedBy>(newOwner.Value)
		);
	}
	
	private static readonly Stream<DropBy, DropItem> toDrop =
		world.Query<DropBy, DropItem>()
			.Has<DropEvent>()
			.Not<EventCancelled>()
			.Not<Visuals>()
			.Stream();
	private static void HandleDrop(ref DropBy droppant, ref DropItem item)
	{
		OwnershipManager.RemoveOwner(item.Value);
		EEvent.Spawn()
			.Add<UnequippingEvent>()
			.Add<Visuals>()
			.Add(new UnequippingBy(droppant.Value))
			.Add(new UnequippingItem(item.Value));
		
		if (!item.Value.TryRead<EcsArea2D>(out var itemHandle)) return;
		if (!droppant.Value.TryRead<Node2D>(out var droppantHandle)) return;
		
		itemHandle.Visible = true;
		itemHandle.SetDeferred("disabled", false);
		itemHandle.SetParent(ItemsManager.DroppedToolsContainer);
		
		itemHandle.GlobalPosition = droppantHandle.GlobalPosition;
	}
}
