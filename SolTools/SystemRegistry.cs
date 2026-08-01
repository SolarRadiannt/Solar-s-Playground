namespace SolTools;
using Godot;

public static class SystemRegistry
{
	public static Node[] All => [
		new Systems.PickupAndDrop(),
		new Systems.EquipAndUnequip()
	];
}