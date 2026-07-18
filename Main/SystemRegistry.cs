namespace Main.Systems;
using Godot;

public static class SystemRegistry
{
	public static Node[] GetAll() => [
		new Systems.ComputeVelocity(),
		new Systems.FootstepsSound(),
	];
}