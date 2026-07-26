namespace SolFramework;

using fennecs;
public struct Transient;
public static class ETransient
{
	private static World world = Core.World;
	public static Entity Spawn() =>
		world.Spawn()
			.Add<Transient>();
	
	public static EntitySpawner Spawner() =>
		world.Entity()
			.Add<Transient>();
}