namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Scheduler;
using SolFramework;

public partial class TransientFlusher : Node, ISystem
{
	public int Priority => SPriority.Flush - 5;
	public void Process(double _) =>
		transientEntities.Despawn();
	
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
	
	private static readonly World world = Core.World;
	private static readonly Stream<Transient> transientEntities = world.Stream<Transient>();
}