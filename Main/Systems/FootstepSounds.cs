namespace Root.Systems;

using Godot;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Core;
using fennecs;
using SolFramework.MoveManager;
using SolFramework.FootstepManager;

public partial class FootstepSounds : Node, ISystem
{
	private static readonly World world = Core.World;
	
	public int Priority => SPriority.Default;
	
	public void Process(double _)
	{
		PlayFootsteps();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
		
	}

	public override void _Ready() => Init();
	
	private static readonly Stream<FootstepOrigin, FootstepMaterial, FootstepSource> footstepEmitter =
		world.Query<FootstepOrigin, FootstepMaterial, FootstepSource>()
		.Has<FootstepEvent>()
		.Stream();
	private void PlayFootsteps() =>
		footstepEmitter.For(
			(ref FootstepOrigin origin, ref FootstepMaterial material, ref FootstepSource source) =>
			{
				GD.Print("Step!");
				GetTree().CurrentScene.GetNode<AudioStreamPlayer>("StepTest").Play();
			});
}