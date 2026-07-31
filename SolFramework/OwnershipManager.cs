namespace SolFramework.Managers;

using System.Linq;
using fennecs;

using SharpResults.Core.Types;
using SharpResults.Types;

using SolFramework;
using SolFramework.Components;

public struct OwnershipChangedEvent;
public struct OwnedEntity;
public struct NewOwner;

public struct PreviouslyOwned;
public struct PreviousOwner;

public enum OwnershipError
{
	AlreadyOwned,
	HasOwner,
	NoOwner,
}
public static class OwnershipManager
{
	private static readonly World world = Core.World;
	
	public static Entity GetOwned(Entity owned) =>
		world.Query()
			.Has<Owning>(owned)
			.Compile().First();
	
	public static Entity GetOwner(Entity owner) =>
		world.Query()
			.Has<OwnedBy>(owner)
			.Compile().First();
	
	public static bool TryGetOwner(Entity owned, out Entity owner) =>
		world.Query()
			.Has<Owning>(owned)
			.Compile().TryFirst(out owner);
	
	public static bool TryGetOwned(Entity owner, out Entity owned) =>
		world.Query()
			.Has<OwnedBy>(owner)
			.Compile().TryFirst(out owned);
	
	public static Entity[] GetOwnedAll(Entity owner) =>
		world.Query()
			.Has<OwnedBy>(owner)
			.Compile().ToArray();
	
	public static Result<Unit, OwnershipError> AddOwner(Entity owner, Entity toOwn)
	{
		if (TryGetOwner(toOwn, out var otherOwner))
			if (owner.ToRaw() == otherOwner.ToRaw())
				return OwnershipError.AlreadyOwned;
			else
				return OwnershipError.HasOwner;
		
		toOwn.Add<OwnedBy>(owner);
		owner.Add<Owning>(toOwn);
		
		return Unit.Default;
	}
	
	public static Result<Entity, OwnershipError> RemoveOwner(Entity owned)
	{
		if (!TryGetOwner(owned, out var owner)) return OwnershipError.NoOwner;
		
		owner.Remove<Owning>(owned);
		owned.Remove<OwnedBy>(owner);
		
		return EEvent.Spawn()
			.Add<OwnedEntity>(owned)
			.Add<PreviousOwner>(owner);
	}
	
	public static Result<Unit, OwnershipError> SetOwner(Entity owner, Entity toOwn)
	{
		if (TryGetOwner(toOwn, out var otherOwner))
		{
			if (owner.ToRaw() == otherOwner.ToRaw())
				return OwnershipError.AlreadyOwned;
			
			otherOwner.Remove<Owning>(toOwn);
			toOwn.Remove<OwnedBy>(otherOwner);
		}
		
		owner.Add<Owning>(toOwn);
		toOwn.Add<OwnedBy>(owner);
		
		return Unit.Default;
	}
}