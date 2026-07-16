namespace Main.Systems;

using Godot;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Core;
using fennecs;
using SolFramework.MoveManager;

public partial class ComputeVelocity : Node, ISystem
{
	private static World world = Core.World;
	
	private static Stream<Velocity> query_compute =
		world.Query<Velocity>().Stream();
	
	public int Priority => SPriority.Transformation + 10;
	
	public void Process(double _)
	{
		query_compute.For(
			static(in Entity entity, ref Velocity vel) =>
			{
				var newVel = Vector2.Zero;
				if (entity.Has<MoveVelocity>())
					newVel += entity.Ref<MoveVelocity>().Value;
				
				
				vel.Value = newVel;
			});
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
}