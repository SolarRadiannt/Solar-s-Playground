using fennecs;
using Godot;

using SolFramework;
using SolFramework.Components;
using SolFramework.Tools;


[GlobalClass]
[InspectorColor(InspectColor.Violet)]
public abstract partial class EcsNode2D : Node2D
{
	protected Entity entity;
	public Entity Entity => entity;
	protected abstract void OnEntityReady();

	public override void _EnterTree()
	{
		if (!entity)
		{
			entity = Core.World.Spawn()
					.Add(new Name(Name));
		}
		
		entity.TryAdd(this);
		entity.TryAdd<Node2D>(this);
		
		OnEntityReady();
	}
	
	public override void _ExitTree()
	{
		if (entity)
		{
			entity.TryRemove<EcsNode2D>();
			entity.TryRemove<Node2D>();
		}
	}
	
	protected override void Dispose(bool disposing)
	{
		if (disposing && entity) entity.Despawn();
		base.Dispose(disposing);
	}
}