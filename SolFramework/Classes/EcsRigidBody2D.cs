using fennecs;
using Godot;

using SolFramework;
using SolFramework.Components;
using SolFramework.Tools;


[GlobalClass]
[InspectorColor(InspectColor.Violet)]
public abstract partial class EcsRigidBody2D : RigidBody2D
{
	protected Entity entity;
	public Entity Entity => entity;
	protected abstract void OnEntityReady();
	
	public override void _EnterTree()
	{
		if (!entity)
			entity = Core.World.Spawn()
						.Add(new Velocity(Vector2.Zero))
						.Add(new Name(Name));
		
		entity.TryAdd(this);
		entity.TryAdd(new Node2DHandle(this));
		
		OnEntityReady();
	}
	public override void _ExitTree()
	{
		if (entity)
		{
			entity.TryRemove<EcsRigidBody2D>();
			entity.TryRemove<Node2DHandle>();
		}
	}
	
	protected override void Dispose(bool disposing)
	{
		if (disposing && entity) entity.Despawn();
		base.Dispose(disposing);
	}
}