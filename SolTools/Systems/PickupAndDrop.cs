namespace SolTools.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;
using SolTools.Components;
using System.Linq;
using Root;
using SolTools.Managers;

public partial class PickupAndDrop : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying + 1;
	public void Process(double delta)
	{
		HandlePickup();
		HandleDrop();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<PickupBy, PickupTool> toPickup =
		world.Query<PickupBy, PickupTool>()
			.Has<PickupEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandlePickup() =>
		toPickup.For(static
		(ref PickupBy newOwner, ref PickupTool tool) =>
		{
			OwnershipManager.SetOwner(newOwner.Value, tool.Value);
			
			if (tool.Value.TryRead<EcsArea2D>(out var toolHandle))
			{
				if (!newOwner.Value.TryRead<Node2DHandle>(out var ownerHandle)) return;
				
				toolHandle.Visible = false;
				toolHandle.SetDeferred("disabled", true);
				toolHandle.SetParent(ownerHandle.Value);
			}
		});
	
	private static readonly Stream<DropBy, DropTool> toDrop =
		world.Query<DropBy, DropTool>()
			.Has<DropEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandleDrop() =>
		toDrop.For(static
		(ref DropBy droppant, ref DropTool tool) =>
		{
			OwnershipManager.RemoveOwner(tool.Value);
			
			if (tool.Value.TryRead<EcsArea2D>(out var toolHandle))
			{
				if (!droppant.Value.TryRead<Node2DHandle>(out var droppantHandle)) return;
				toolHandle.Visible = true;
				toolHandle.SetDeferred("disabled", false);
				toolHandle.SetParent(ToolsManager.DroppedToolsContainer);
				
				toolHandle.GlobalPosition = droppantHandle.Value.GlobalPosition; 
			}
		});
}
