namespace Root.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;
using Root.Components;

public partial class ItemsPickup : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	public void Process(double delta)
	{
		PickupHandler();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<EcsArea2D> pickupables =
		world.Query<EcsArea2D>()
			.Has<Pickupable>()
			.Not<OwnedBy>(Entity.Any)
			.Stream();
	private static void PickupHandler() =>
		pickupables.For(static
		(ref EcsArea2D area) =>
		{
			
		});
}
