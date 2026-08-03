namespace Root;
using Godot;

public static class SystemRegistry
{
	public static Node[] All => [
		new Systems.ComputeVelocity(),
		new Systems.FootstepSounds(),
		new Systems.ItemsPickup(),
	];
}