namespace Root.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.Managers;

using GodotUtilities;
using GodotUtilities.AudioManagement;


public partial class FootstepSounds : Node, ISystem
{
	private static readonly World world = Core.World;
	
	public int Priority => SPriority.Default;
	
	public void Process(double _)
	{
		// PlayFootsteps();
	}
	
	public void Init()
	{
		Scheduler.RegisterSystem(this);
	}

	public override void _Ready() => Init();
	
	// private static readonly StringName[] footsteps = [
	// 	AudioManager.SfxName.Footstep1,
	// 	AudioManager.SfxName.Footstep2,
	// 	AudioManager.SfxName.Footstep3,
	// ];
	
	// private static readonly Stream<FootstepOrigin, FootstepMaterial, FootstepSource> footstepEvents =
	// 	world.Query<FootstepOrigin, FootstepMaterial, FootstepSource>()
	// 	.Has<FootstepEvent>()
	// 	.Stream();
	// private void PlayFootsteps() =>
	// 	footstepEvents.For(static
	// 		(ref FootstepOrigin origin, ref FootstepMaterial material, ref FootstepSource source) =>
	// 		{
	// 			var selected = MathUtil.PickRandom(footsteps);
	// 			AudioManager.Instance.PlaySfx2D(selected, origin.Value);
	// 		});
}