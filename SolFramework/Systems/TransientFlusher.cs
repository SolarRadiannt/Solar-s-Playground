namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Scheduler;
using SolFramework.Core;
using SolFramework.ETransient;

public partial class TransientFlusher : Node, ISystem
{
	public int Priority => SPriority.Flush;
	public void Process(double _)
	{
		query_evententity.Despawn();
	}
	public static World world = Core.World;
	public static Stream<Transient> query_evententity = query_evententity = world.Stream<Transient>();
	
	public void Init()
	{
		Scheduler.RegisterSystem(this.Process, Priority);
	}

	public override void _Ready()
	{
		Init();
	}
}