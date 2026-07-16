namespace SolFramework.EEvents
{
	using fennecs;
	using SolFramework.Core;
	using SolFramework.ETransient;
	
	public struct EventEntity;
	public struct EventCancelled;
	public static class EEvent
	{
		public static Entity Spawn()
		{
			return ETransient.Spawn()
				.Add<EventEntity>();
		}
	}
}