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
using System.Linq;
using SharpResults.Core;


public partial class ItemsPickup : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	public static readonly SolPointQuery query = new()
    {
		CollideWithAreas = true
	};
	
	public void Process(double delta)
	{
		if (!Input.IsActionJustPressed("pickup")) return;
		
		var mousePos = MainGame.Instance.GetGlobalMousePosition();
		
		GD.Print("pickup clicked");
		var results = SolSpatial2D.IntersectPoint(mousePos, query);
		GD.Print(results);
		
		foreach (var found in results)
			if (found.Collider is EcsArea2D area)
			{
				var entity = area.Entity;
				if (!entity.HasAll<Pickupable, Item>()) return;
				if (entity.Has<OwnedBy>(Entity.Any)) return;
				
				ItemsManager.Pickup(entity, MainGame.Player.Entity);
				GD.Print($"{entity.GetName()} has been picked up");
				break;
			}
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
}
