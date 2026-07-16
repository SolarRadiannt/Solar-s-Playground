namespace SolFramework.EEvents;

using fennecs;
using SolFramework.ETransient;
using SolFramework.Core;

public struct EventEntity;
public struct EventCancelled;
public static class EEvent
{
	public static Entity Spawn()
	{
		return ETransient.Spawn()
			.Add<EventEntity>();
	}
	public static EntitySpawner Spawner()
	{
		return ETransient.Spawner()
			.Add<EventEntity>();
	}
}
