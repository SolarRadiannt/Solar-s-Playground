namespace PlayerManager.Systems;

using Godot;
using System;
using fennecs;
using Root;

using SolFramework.Components;
using SolFramework.Core;
using SolFramework.Scheduler;
using SolFramework.MoveManager;
using GodotUtilities;


public partial class LookAtCursor : Node, ISystem
{
	private static readonly World world = Core.World;
	public int Priority => SPriority.Default;
	public void Process(double delta)
	{
		LookToCursor();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
	public override void _Ready() => Init();
	
	private static Stream<ECSCharBody2D> controlledToLook =
		world.Query<ECSCharBody2D>()
			.Has<Player>()
			.Stream();
	private static void LookToCursor() =>
		controlledToLook.For(
		static (ref ECSCharBody2D body) =>
		{
			body.LookAt(Main.Instance.GetGlobalMousePosition());
		});
}
