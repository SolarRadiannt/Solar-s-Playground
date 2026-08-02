namespace SolItems;
using Godot;

public static class SystemRegistry
{
	public static Node[] All => [
		new Systems.PickupAndDrop(),
		new Systems.EquipAndUnequip(),
		new Systems.CheckPickupOwnership(),
	];
}