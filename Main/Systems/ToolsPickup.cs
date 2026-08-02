namespace Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;
using Root.Components;

public partial class ToolsPickup : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	public void Process(double delta)
	{
		
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<EcsArea2D> pickpables =
		world.Query<EcsArea2D>()
			.Has<Pickupable>()
			.Not<OwnedBy>(Entity.Any)
			.Stream();
	private static void PickupHandler()
	{
		
	}
}
