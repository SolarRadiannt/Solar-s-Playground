namespace Systems;

using Godot;
using System;
using fennecs;

using SolFramework;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Managers;


public partial class ToolsPickup : Node, ISystem
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
	
}
