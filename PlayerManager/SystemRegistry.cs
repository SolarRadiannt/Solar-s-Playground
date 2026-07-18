namespace PlayerManager.Systems;
using Godot;

public static class SystemRegistry
{
	public static Node[] GetAll() => [
		new Systems.MoveDirectionInput()
	];
}