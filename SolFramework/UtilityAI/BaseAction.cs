namespace SolFramework.UtilityAI;

using fennecs;

public abstract class BaseAction
{
	public abstract float Score(Entity entity);
	public abstract bool CanExecute(Entity entity);
	public abstract void Start(Entity entity);
	public abstract void Stop(Entity entity);
}