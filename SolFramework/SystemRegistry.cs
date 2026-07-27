namespace SolFramework;
using Godot;

public static class SystemRegistry
{
	public static Node[] All => [
		new Systems.ApplyVelocity(),
		new Systems.FootstepsEmitter(),
		new Systems.HealthApply(),
		new Systems.Movement(),
		new Systems.TimerTicker(),
		new Systems.TransientFlusher(),
		new Systems.MovingChecker(),
		new Systems.LookToMoveDirection(),
		new Systems.Destruction(),
		new Systems.MoveDirectionInput(),
		new Systems.ActualVelCalc()
	];
}