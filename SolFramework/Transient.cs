namespace SolFramework.ETransient
{
	using fennecs;
	using SolFramework.Core;
	public struct Transient;
	public static class ETransient
	{
		private static World world = Core.World;
		public static Entity Spawn()
		{
			return world.Spawn()
				.Add<Transient>();
		}
		public static EntitySpawner Spawner()
		{
			return world.Entity()
				.Add<Transient>();
		}
	}
}