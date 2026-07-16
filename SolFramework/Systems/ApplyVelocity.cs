namespace SolFramework.Systems;

using fennecs;
using Godot;

using SolFramework.Components;
using SolFramework.Core;
using SolFramework.Scheduler;

public partial class ApplyVelocity : ISystem
{
	public int Priority => SPriority.Applying + 5;
	public void Process(double _)
	{
		query_characterbody.For(
			static (ref ECSCharBody2D body, ref Velocity vel) =>
			{
				body.Velocity = vel.Value;
			});
	}
	
	private static World world = Core.World;
	private static Stream<ECSCharBody2D, Velocity> query_characterbody =
		world.Stream<ECSCharBody2D, Velocity>();
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}
}