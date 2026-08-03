namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.UtilityAI.Components;
using SolFramework.UtilityAI;
using System.Collections.Generic;
using System.Linq;
using System;
using SolFramework.Components;

public partial class ActionsExecutor : Node, ISystem
{
	private static readonly World world = Core.World;
	private static readonly IdleAction _idleAction = new();
	
	public int Priority => SPriority.Transformation;
	public void Process(double delta)
	{
		AgentRemover();
		AgentUpdater();
	}
	
	public void Init()
	{
		Scheduler.RegisterPhysicsSystem(this);
	}
	public override void _Ready() => Init();
	
	private static BaseAction GetBestAction(Entity entity, BaseAction[] actions)
	{
		BaseAction best = null;
		float highestScore = float.MinValue;

		for (int i = 0; i < actions.Length; i++)
		{
			var action = actions[i];
			if (!action.CanRun(entity)) continue;

			float score = action.Score(entity);
			if (score > highestScore)
			{
				highestScore = score;
				best = action;
			}
		}
		
		return best ?? _idleAction;
	}
	
	public static Dictionary<Entity, BaseAction> entityActiveActionMap = new();
	
	private static Stream<AgentActions> destroyedAgents = 
		world.Query<AgentActions>()
		.Has<Destroy>()
		.Stream();
	private static void AgentRemover() =>
		destroyedAgents.For(static
		(in Entity entity, ref AgentActions _) => entityActiveActionMap.Remove(entity));
	
	private static Stream<AgentActions> agentsToUpdate =
		world.Query<AgentActions>()
		.Not<AgentDisabled>()
		.Stream();
	private static void AgentUpdater() =>
	agentsToUpdate.For(static
		(in Entity entity, ref AgentActions actions) =>
		{
			 if (!entityActiveActionMap.TryGetValue(entity, out var currentAction))
				currentAction = null;
			
			var chosenAction = GetBestAction(entity, actions.Value);
			if (chosenAction == currentAction) return;
			
			currentAction?.Stop(entity);
			
			chosenAction.Start(entity);
			entityActiveActionMap[entity] = chosenAction;
		});
}
