namespace Root.Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;
using Root.Components;
using GodotUtilities;
using SolItems.Components;
using SolItems.Managers;


public partial class ItemsPickup : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	public void Process(double delta)
	{
		if (!Input.IsActionJustPressed("pickup")) return;
		
		GD.Print("pickup clicked");
		var space_state = MainGame.Instance.GetWorld2D().DirectSpaceState;
		
		var query = new PhysicsPointQueryParameters2D();
		query.Position = MainGame.Instance.GlobalPosition;
		var result = space_state.IntersectPoint(query)
		if (result is EcsArea2D area)
		{
			var entity = area.Entity;
			if (entity.HasAll<Pickupable, Item>()) return;
			if (entity.Has<OwnedBy>(Entity.Any)) return;
			
			ItemsManager.Pickup(entity, MainGame.Player.Entity);
			GD.Print($"{entity.GetName()} has been picked up");
		}
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
}
