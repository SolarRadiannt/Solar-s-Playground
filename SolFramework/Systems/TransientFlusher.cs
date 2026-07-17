namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Scheduler;
using SolFramework.Core;
using SolFramework.ETransient;
using SolFramework.Components;


public partial class TransientFlusher : Node, ISystem
{
	public int Priority => SPriority.Flush;
	public void Process(double _)
	{
		transientEntities.Despawn();
		destroyEntities.Despawn();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this.Process, Priority);
	}

	public override void _Ready() => Init();
	
	private static readonly World world = Core.World;
	private static readonly Stream<Transient> transientEntities = world.Stream<Transient>();
	private static readonly Stream<Destroy> destroyEntities = world.Stream<Destroy>();
}