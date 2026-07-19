namespace Root.Systems;

using Godot;
using SolFramework.Components;
using SolFramework.Scheduler;
using SolFramework.Core;
using fennecs;
using SolFramework.MoveManager;
using SolFramework.FootstepManager;
using GodotUtilities.AudioManagement;
using GodotUtilities;

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
	
	private static readonly StringName[] concreteFootsteps = [
		AudioManager.SfxName.FConcrete1,
		AudioManager.SfxName.FConcrete2,
		AudioManager.SfxName.FConcrete3,
		AudioManager.SfxName.FConcrete4,
		AudioManager.SfxName.FConcrete5,
		AudioManager.SfxName.FConcrete6,
		AudioManager.SfxName.FConcrete7,
		AudioManager.SfxName.FConcrete8,
		AudioManager.SfxName.FConcrete9,
		AudioManager.SfxName.FConcrete10,
		AudioManager.SfxName.FConcrete11,
		AudioManager.SfxName.FConcrete12,
		AudioManager.SfxName.FConcrete13,
		AudioManager.SfxName.FConcrete14,
		AudioManager.SfxName.FConcrete15,
		AudioManager.SfxName.FConcrete16,
	];
	
	private static readonly Stream<FootstepOrigin, FootstepMaterial, FootstepSource> footstepEmitter =
		world.Query<FootstepOrigin, FootstepMaterial, FootstepSource>()
		.Has<FootstepEvent>()
		.Stream();
	private void PlayFootsteps() =>
		footstepEmitter.For(
			(ref FootstepOrigin origin, ref FootstepMaterial material, ref FootstepSource source) =>
			{
				GD.Print("Step!");
				var selectedFootstep = MathUtil.PickRandom(concreteFootsteps);
				AudioManager.Instance.PlaySfx2D(selectedFootstep, origin.Value);
			});
}