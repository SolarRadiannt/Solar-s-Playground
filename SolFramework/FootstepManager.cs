namespace SolFramework.FootstepManager;

using fennecs;
using Godot;

using SolFramework.EEvents;
using SolFramework.TickTimer;
using SolFramework.Core;
public struct FootstepEmitter;
public struct FootstepEvent;

public record struct FootstepMaterial(string Value);
public record struct FootstepOrigin(Vector2 Value);
public record struct FootstepSource(Entity Value);

public record struct FootstepTimer(TickTimer Value);
public record struct FootstepBaseRate(float Value);

static class FootstepManager
{
	public static Entity ApplyFootstep(Entity entity, float stepRate) =>
			entity
				.Add(new FootstepTimer(new TickTimer(stepRate)))
				.Add(new FootstepBaseRate(stepRate))
				.Add<FootstepEmitter>();
	
	public static Entity SetBaseStepRate(Entity entity, float baseStepRate)
	{
		if (entity.Has<FootstepTimer>())
		{
			entity.Ref<FootstepBaseRate>().Value = baseStepRate;
		}
		else
			GD.PushWarning(Core.GetName(entity), "does not have footstep timer!");
		
		return entity;
	}
	
	public static Entity EmitFootstep(Vector2 origin, string material, Entity source) =>
		EEvent.Spawn()
			.Add(new FootstepOrigin(origin))
			.Add(new FootstepMaterial(material))
			.Add(new FootstepSource(source))
			.Add<FootstepEvent>();
	
}
