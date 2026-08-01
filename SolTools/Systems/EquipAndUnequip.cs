namespace Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;
using SolTools.Components;

public partial class EquipAndUnequip : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Default;
	public void Process(double delta)
	{
		
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<EquippingBy, EquippingTool> toEquip = 
		world.Query<EquippingBy, EquippingTool>()
			.Has<EquipEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandleEquip() =>
		toEquip.For(static
		(ref EquippingBy owner, ref EquippingTool tool) =>
		{
			if (!owner.Value.TryRead<Node2DHandle>(out var ownerHandle)) return;
			if (!tool.Value.TryRead<EcsArea2D>(out var toolHandle)) return;
			
			toolHandle.Visible = true;
			tool.Value.Add<EquippedBy>(owner.Value);
		});
	
	private static readonly Stream<UnequippingBy, UnequippingTool> toUnequip =
		world.Query<UnequippingBy, UnequippingTool>()
			.Has<UnequipEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandleUnequip() =>
		toUnequip.For(static
		(ref UnequippingBy owner, ref UnequippingTool tool) =>
		{
			if (!owner.Value.TryRead<Node2DHandle>(out var ownerHandle)) return;
			if (!tool.Value.TryRead<EcsArea2D>(out var toolHandle)) return;
			
			toolHandle.Visible = false;
			tool.Value.Remove<EquippedBy>(owner.Value);
		});
}
