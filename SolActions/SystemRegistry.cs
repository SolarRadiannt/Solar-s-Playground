using Godot;

namespace SolActions;

public static class SystemRegistry
{
	public static Node[] All => [
		new Systems.HandleWanderer()
	];
}