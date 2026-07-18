namespace SolFramework.Systems;
using Godot;

public static class SystemRegistry
{
	public static Node[] GetAll() => [
		new Systems.ApplyVelocity(),
		new Systems.FootstepsEmitter(),
		new Systems.HealthApply(),
		new Systems.Movement(),
		new Systems.TimerTicker(),
		new Systems.TransientFlusher(),
	];
}