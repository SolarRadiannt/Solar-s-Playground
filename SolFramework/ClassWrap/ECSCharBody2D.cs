using fennecs;
using Godot;

using SolFramework.Core;

[GlobalClass]
public abstract partial class ECSCharBody2D : CharacterBody2D
{
	protected Entity entity;
	public Entity Entity => entity;
	
	protected abstract void OnEntityReady();
	
	public override void _EnterTree()
	{
		if (!entity)
		{
			entity = Core.World.Spawn();
		}
		
		entity.Add(this);
		OnEntityReady();
	}
	public override void _ExitTree() =>
		entity.Remove<ECSCharBody2D>();

	protected override void Dispose(bool disposing)
	{
		if (disposing) entity.Despawn();
		base.Dispose(disposing);
	}

	public override void _PhysicsProcess(double _)
	{
		MoveAndSlide();
	}
}