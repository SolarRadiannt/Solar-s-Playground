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


public partial class RifleEquipper : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	
	public void Process(double delta)
	{
		rifleToUnEquip.For(HandleUnequip);
		rifleToEquip.For(HandleEquip);
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
	
	private static readonly Stream<Item> rifleToEquip =
		world.Query<Item>()
			.Has<OwnedBy>(MainGame.Player.Entity)
			.Has<FirearmType<Rifle>>()
			.Not<EquippedBy>()
			.Stream();
	private static void HandleEquip(in Entity entity, ref Item _)
	{
		if (!Input.IsActionJustPressed("primary_equip")) return;
		GD.Print("equip requested");
		GD.Print(ItemsManager.Equip(entity));
	}
	private static readonly Stream<Item> rifleToUnEquip =
		world.Query<Item>()
			.Has<OwnedBy>(MainGame.Player.Entity)
			.Has<FirearmType<Rifle>>()
			.Has<EquippedBy>()
			.Stream();
	private static void HandleUnequip(in Entity entity, ref Item _)
	{
		if (!Input.IsActionJustPressed("primary_equip")) return;
		GD.Print("unequip requested");
		ItemsManager.Unequip(entity);
	}
}
