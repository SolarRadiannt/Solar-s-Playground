namespace SolTools.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;
using SolTools.Components;
using SolTools.Managers;


public partial class EquipAndUnequip : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Applying;
	public void Process(double delta)
	{
		HandleEquip();
		HandleUnequip();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<EquippingBy, EquippingTool> toEquip = 
		world.Query<EquippingBy, EquippingTool>()
			.Has<EquippingEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandleEquip() =>
		toEquip.For(static
		(in Entity eevent, ref EquippingBy owner, ref EquippingTool tool) =>
		{
			if (!owner.Value.TryRead<Node2DHandle>(out var ownerHandle)) return;
			if (!tool.Value.TryRead<EcsArea2D>(out var toolHandle)) return;
			
			if (eevent.Has<SwapEquip>() && ToolsManager.TryGetEquipped(owner.Value, out var equippedTool))
			{
				if (equippedTool.TryRead<EcsArea2D>(out var oldToolHandle))
					oldToolHandle.Visible = false;
				
				equippedTool.Remove<EquippedBy>();
				owner.Value.Remove<EquippedTool>();
			}
			
			toolHandle.Visible = true;
			tool.Value.Add(new EquippedBy(owner.Value));
			owner.Value.Add(new EquippedTool(tool.Value));
		});
	
	private static readonly Stream<UnequippingBy, UnequippingTool> toUnequip =
		world.Query<UnequippingBy, UnequippingTool>()
			.Has<UnequippingEvent>()
			.Not<EventCancelled>()
			.Stream();
	private static void HandleUnequip() =>
		toUnequip.For(static
		(ref UnequippingBy owner, ref UnequippingTool tool) =>
		{
			if (tool.Value.TryRead<EcsArea2D>(out var toolHandle))
				toolHandle.Visible = false;
			
			
			tool.Value.Remove<EquippedBy>();
			owner.Value.TryRemove<EquippedTool>();
		});
}
