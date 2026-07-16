namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework.Core;
using SolFramework.Components;
using SolFramework.Scheduler;
using System;


public partial class Movement : Node, ISystem
{
	private static World world = Core.World;
	public int Priority => SPriority.Highest;
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public void Process(double delta)
	{
		
	}
	public override void _Ready()
	{
		Init();
	}
}