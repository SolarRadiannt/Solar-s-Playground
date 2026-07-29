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
		destroyBody.Raw(static bodies => {
			foreach (ref var body in bodies.Span)
				body.QueueFree();
		});
		destroyRigidBody.Raw(static rigidBodies => {
			foreach (ref var body in rigidBodies.Span)
				body.QueueFree();
		});
		destroyNode2d.Raw(static nodes => {
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
			.Not<EcsCharBody2D>()
			.Not<EcsNode2D>()
			.Stream();
	private static readonly Stream<EcsRigidBody2D> destroyRigidBody=
		world.Query<EcsRigidBody2D>()
			.Has<Destroy>()
			.Stream();
	private static readonly Stream<EcsCharBody2D> destroyBody =
		world.Query<EcsCharBody2D>()
			.Has<Destroy>()
			.Stream();
	private static readonly Stream<EcsNode2D> destroyNode2d =
		world.Query<EcsNode2D>()
			.Has<Destroy>()
			.Stream();
}