namespace Main;

using Godot;
using System;
using SolFramework;
using SolFramework.Scheduler;


public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Scheduler.ProcessAll(delta);
	}
}
