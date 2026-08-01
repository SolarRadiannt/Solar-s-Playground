namespace Systems;

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


public partial class PickupAndDrop : Node, ISystem
{
	// put a dedicated droped tools later
	public static readonly Node2D DroppedToolsContainer = MainGame.Instance; 
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
	
	private static readonly Stream<PickedUpBy, PickedUpTool> toPickup =
		world.Query<PickedUpBy, PickedUpTool>()
			.Has<PickupEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandlePickup() =>
		toPickup.For(static
		(ref PickedUpBy newOwner, ref PickedUpTool tool) =>
		{
			if (!newOwner.Value.TryRead<Node2DHandle>(out var ownerHandle)) return;
			if (!tool.Value.TryRead<EcsArea2D>(out var toolHandle)) return;
			
			toolHandle.Visible = false;
			toolHandle.SetDeferred("disabled", true);
			toolHandle.SetParent(ownerHandle.Value);
		});
	
	private static readonly Stream<DroppedBy, DroppedTool> toDrop =
		world.Query<DroppedBy, DroppedTool>()
			.Has<DropEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandleDrop() =>
		toDrop.For(static
		(ref DroppedBy droppant, ref DroppedTool tool) =>
		{
			if (!droppant.Value.TryRead<Node2DHandle>(out var droppantHandle)) return;
			if (!tool.Value.TryRead<EcsArea2D>(out var toolHandle)) return;
			
			toolHandle.Visible = true;
			toolHandle.GlobalPosition = droppantHandle.Value.GlobalPosition; 
			toolHandle.SetDeferred("disabled", false);
			toolHandle.SetParent(DroppedToolsContainer);
		});
}
