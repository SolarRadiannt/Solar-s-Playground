using Godot;
using SolFramework.Components;
using SolFramework.Managers;
using SolFramework.Tools;
using SolFramework.UtilityAI.Components;

using SolActions;

public partial class DumbAI : EcsCharBody2D
{
	protected override void OnEntityReady()
	{
		GD.Print("dumb ai spawned");
		
		entity.Add<Grounded>();
		MoveManager.ApplyMovement(entity, 400);
		FootstepManager.ApplyFootstep(entity, 3f);
		HealthManager.ApplyHealth(entity, 100f);
		
		entity.Add(new WanderCooldown(new TickTimer(5f)));
		entity.Add(new AgentActions([
			ActionsReg.Wander,
			ActionsReg.Idle
		]));
	}
	
	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}
}
