using Godot;
using System;

using SolFramework;
using SolFramework.Components;
using SolFramework.Managers;
using System.Collections.Generic;
using SolFramework.UtilityAI.Components;

public partial class DumbAi : EcsCharBody2D
{
	protected override void OnEntityReady()
	{
		entity.Add<Grounded>();
		MoveManager.ApplyMovement(entity, 400);
		FootstepManager.ApplyFootstep(entity, 3f);
		HealthManager.ApplyHealth(entity, 100f);
		
		entity.Add(new AgentActions([]));
	}

}
