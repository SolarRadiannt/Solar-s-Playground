namespace SolFramework.EEvents;

using fennecs;
using SolFramework.ETransient;
using SolFramework.Core;

public struct EventEntity;
public struct EventCancelled;
public static class EEvent
{
	public static Entity Spawn(string name) =>
		ETransient.Spawn(name)
			.Add<EventEntity>();
	
	public static EntitySpawner Spawner(string name) =>
		ETransient.Spawner(name)
			.Add<EventEntity>();
}
