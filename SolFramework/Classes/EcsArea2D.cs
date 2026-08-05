using fennecs;
using Godot;

using SolFramework;
using SolFramework.Components;
using SolFramework.Tools;


[GlobalClass]
[InspectorColor(InspectColor.Violet)]
public abstract partial class EcsArea2D : Area2D
{
	protected Entity entity;
	public Entity Entity => entity;
	protected abstract void OnEntityReady();
	
	public override void _EnterTree()
	{
		if (entity) return;

		entity = Core.World.Spawn()
			.Add(new Velocity(Vector2.Zero))
			.Add(new Name(Name))
			.Add<Character>();
		
		entity.Add(this);
		entity.Add<Node2D>(this);
		
		OnEntityReady();
	}
	public override void _ExitTree()
	{
		if (entity)
		{
			entity.TryRemove<EcsArea2D>();
			entity.TryRemove<Node2D>();
		}
	}
	
	protected override void Dispose(bool disposing)
	{
		if (disposing && entity) entity.Despawn();
		base.Dispose(disposing);
	}
}