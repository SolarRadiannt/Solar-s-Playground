namespace SolItems.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;

using SolItems.Components;
using SolItems.Managers;


public partial class EquipAndUnequip : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying;
	public void Process(double delta)
	{
		toEquip.For(HandleEquip);
		toUnequip.For(HandleUnequip);
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<EquippingBy, EquippingItem> toEquip = 
		world.Query<EquippingBy, EquippingItem>()
			.Has<EquippingEvent>()
			.Not<EventCancelled>()
			.Not<Visuals>()
			.Stream();
	private static void HandleEquip(in Entity eevent, ref EquippingBy owner, ref EquippingItem item)
	{
		if (!ItemsManager.TryGetOwner(item.Value, out var currentOwner) || !currentOwner.Equals(owner.Value))
		{
			eevent.Add<EventCancelled>();
			return;
		}
		
		if (eevent.Has<SwapEquip>() && ItemsManager.TryGetEquipped(owner.Value, out var equippedItem))
		{
			if (!ItemsManager.TryGetEquippant(equippedItem, out var equippant) || !equippant.Equals(owner.Value))
			{
				eevent.Add<EventCancelled>();
				return;
			}
			if (equippedItem.TryRead<EcsArea2D>(out var oldItemHandle))
				oldItemHandle.Visible = false;
			
			equippedItem.Remove<EquippedBy>();
			owner.Value.Remove<EquippedItem>();
		}
		
		item.Value.Add(new EquippedBy(owner.Value));
		owner.Value.Add(new EquippedItem(item.Value));
		
		if (!item.Value.TryRead<EcsArea2D>(out var itemHandle)) return;
		itemHandle.Visible = true;
	}
	
	private static readonly Stream<UnequippingBy, UnequippingItem> toUnequip =
		world.Query<UnequippingBy, UnequippingItem>()
			.Has<UnequippingEvent>()
			.Not<EventCancelled>()
			.Not<Visuals>()
			.Stream();
	private static void HandleUnequip(in Entity eevent, ref UnequippingBy owner, ref UnequippingItem item)
	{
		if (!ItemsManager.TryGetOwner(item.Value, out var currentOwner) || !currentOwner.Equals(owner.Value))
		{
			eevent.Add<EventCancelled>();
			return;
		}
		if (!ItemsManager.TryGetEquipped(owner.Value, out var equipped) || !equipped.Equals(item.Value))
		{
			eevent.Add<EventCancelled>();
			return;
		}
		
		
		if (item.Value.TryRead<EcsArea2D>(out var toolHandle))
			toolHandle.Visible = false;
		
		item.Value.Remove<EquippedBy>();
		owner.Value.TryRemove<EquippedItem>();
	}
}
