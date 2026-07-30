namespace SolProjectiles;
using Godot;

public static class SystemRegistry
{
	public static Node[] All => [
		new Systems.FarDestroyer(),
		new Systems.HitDetection(),
		new Systems.ProjectileSpawner(),
	];
}