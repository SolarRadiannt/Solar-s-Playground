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
public record struct FootstepStride(float Value);

static class FootstepManager
{
	public static Entity ApplyFootstep(Entity entity, float stride) =>
			entity
				.Add(new FootstepTimer(new TickTimer(0.25f, true)))
				.Add(new FootstepStride(stride))
				.Add<FootstepEmitter>();
	
	public static Entity SetBaseStepRate(Entity entity, float baseStepRate)
	{
		if (entity.Has<FootstepStride>())
			entity.Ref<FootstepStride>().Value = baseStepRate;
		else
			GD.PushWarning(Core.GetName(entity), "does not have footstep stride!");
		
		return entity;
	}
	
	public static Entity EmitFootstep(Vector2 origin, string material, Entity source) =>
		EEvent.Spawn($"{Core.GetName(source)}'s Footstep")
			.Add(new FootstepOrigin(origin))
			.Add(new FootstepMaterial(material))
			.Add(new FootstepSource(source))
			.Add<FootstepEvent>();
	
}
