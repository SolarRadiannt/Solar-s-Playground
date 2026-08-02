namespace SolItems.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;

using SolItems;
using SolItems.Components;

// Default system that enforce no stealing.
public partial class CheckPickupOwnership : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Interception;
	public void Process(double delta)
	{
		CheckOwnership();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	private static readonly Stream<PickupItem, PickupBy> toCheckIfOwned = 
		world.Query<PickupItem, PickupBy>()
			.Has<PickupEvent>()
			.Not<EventCancelled>()
			.Stream();
	public static void CheckOwnership() =>
		toCheckIfOwned.For(static
		(in Entity eevent, ref PickupItem item, ref PickupBy newOwner) =>
		{
			if (OwnershipManager.TryGetOwner(item.Value, out var existingOwner))
			{
				eevent
					.Add<PickupAlreadyOwned>()
					.Add<EventCancelled>();
			}
		});
}
