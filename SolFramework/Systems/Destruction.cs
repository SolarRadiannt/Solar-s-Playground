namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Scheduler;
using SolFramework;
using SolFramework.Components;

public partial class Destruction : Node, ISystem
{
	public int Priority => SPriority.Flush;
	public void Process(double _)
	{
		destroyHandles.Raw(static nodes => {
			foreach (ref var node in nodes.Span)
				node.QueueFree();
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
		world.Query<Destroy>()
			.Not<Node2D>()
			.Stream();
	private static readonly Stream<Node2D> destroyHandles=
		world.Query<Node2D>()
			.Has<Destroy>()
			.Stream();
}