namespace SolFramework;

using fennecs;

public struct EventEntity;
public struct EventCancelled;
public static class EEvent
{
	public static Entity Spawn() =>
		ETransient.Spawn()
			.Add<EventEntity>();
	
	public static EntitySpawner Spawner() =>
		ETransient.Spawner()
			.Add<EventEntity>();
}
