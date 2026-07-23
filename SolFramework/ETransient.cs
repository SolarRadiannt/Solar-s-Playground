#nullable enable

namespace SolFramework.ETransient;

using fennecs;
using SolFramework.Core;
using SolFramework.Components;
public struct Transient;
public static class ETransient
{
	private static readonly World world = Core.World;
	public static Entity Spawn(string name) =>
		world.Spawn()
			.Add(new Name(name))
			.Add<Transient>();
	
	public static EntitySpawner Spawner(string name) =>
		world.Entity()
			.Add(new Name(name))
			.Add<Transient>();
}