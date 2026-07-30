using fennecs;
using Godot;

using SolFramework;
using SolFramework.Components;
using SolFramework.Tools;


[GlobalClass]
[InspectorColor(InspectColor.Violet)]
public abstract partial class EcsCharBody2D : CharacterBody2D
{
	protected Entity entity;
	public Entity Entity => entity;
	protected abstract void OnEntityReady();
	
	
	public override void _EnterTree()
	{
		if (!entity)
			entity = Core.World.Spawn()
						.Add(new Velocity(Vector2.Zero))
						.Add(new Name(Name))
						.Add<Character>();
		
		if (!entity.Has<EcsCharBody2D>())
			entity.Add(this);
		
		OnEntityReady();
	}
	public override void _ExitTree()
	{
		if (entity && entity.Has<EcsCharBody2D>())
			entity.Remove<EcsCharBody2D>();
	}
	
	protected override void Dispose(bool disposing)
	{
		if (disposing && entity) entity.Despawn();
		base.Dispose(disposing);
	}
}