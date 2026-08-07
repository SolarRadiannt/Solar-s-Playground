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


public partial class FirearmShooter : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Action;
	
	public void Process(double delta)
	{
		
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
}
