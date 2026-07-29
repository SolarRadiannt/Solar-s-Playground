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

	public void Init()
	{
		bool wasMissingEntity = false;

		if (!entity)
		{
			entity = Core.World.Spawn()
					.Add(new Name(Name));
			wasMissingEntity = true;		
		}
		
		if (!entity.Has<EcsNode2D>())
			entity.Add(this);
		
		if (wasMissingEntity)
			OnEntityReady();
	}

	public override void _EnterTree() => Init();
	
	public override void _ExitTree()
	{
		if (entity && entity.Has<EcsNode2D>())
			entity.Remove<EcsNode2D>();
	}
	
	protected override void Dispose(bool disposing)
	{
		if (disposing && entity) entity.Despawn();
		base.Dispose(disposing);
	}
}