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
		
		if (!entity.Has<EcsRigidBody2D>())
			entity.Add(this);
		
		if (!entity.Has<Node2DHandle>())
			entity.Add(new Node2DHandle(this));
		
		OnEntityReady();
	}
	public override void _ExitTree()
	{
		if (entity && entity.Has<EcsRigidBody2D>())
			entity.Remove<EcsRigidBody2D>();
	}
	
	protected override void Dispose(bool disposing)
	{
		if (disposing && entity) entity.Despawn();
		base.Dispose(disposing);
	}
}