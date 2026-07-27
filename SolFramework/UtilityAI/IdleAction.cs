namespace SolFramework.UtilityAI;
using fennecs;
using SolFramework.UtilityAI.Components;

public class IdleAction : BaseAction
{
	public override bool CanExecute(Entity entity) => true;
	public override float Score(Entity entity) => 0.1f;
	public override void Start(Entity entity)
	{
		if (!entity.Has<AgentIdle>())
			entity.Add<AgentIdle>();
	}
	public override void Stop(Entity entity)
	{
		if (entity.Has<AgentIdle>())
			entity.Remove<AgentIdle>();
	}
}