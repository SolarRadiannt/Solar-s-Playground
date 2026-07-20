namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Scheduler;
using SolFramework.Core;
using SolFramework.ETransient;
using SolFramework.Components;
using System;
using System.Linq;

public partial class Destruction : Node, ISystem
{
	public int Priority => SPriority.Flush;
	public void Process(double _)
	{
		destroyBody.Raw(static bodies => {
			foreach (ref var body in bodies.Span)
				body.QueueFree();
		});

		toDestroy.Despawn();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
	
	private static readonly World world = Core.World;
	private static readonly Stream<Destroy> toDestroy =
		world.Stream<Destroy>();
	
	private static readonly Stream<ECSCharBody2D> destroyBody =
		world.Query<ECSCharBody2D>()
		.Has<Destroy>()
		.Stream();
}