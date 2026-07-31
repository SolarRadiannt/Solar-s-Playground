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
public struct OldOwner;

public enum OwnershipError
{
	AlreadyOwned,
	HasOwner,
	NoOwner,
}

public static class OwnershipManager
{
	private static readonly World world = Core.World;
	
	public static Entity GetOwned(Entity owner) =>
		world.Query()
			.Has<Owning>(owner)
			.Compile().First();
	
	public static Entity GetOwner(Entity owned) =>
		world.Query()
			.Has<OwnedBy>(owned)
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
	
	public static Result<Entity, OwnershipError> AddOwner(Entity owner, Entity toOwn)
	{
		if (TryGetOwner(toOwn, out var otherOwner))
			if (owner.ToRaw() == otherOwner.ToRaw())
				return OwnershipError.AlreadyOwned;
			else
				return OwnershipError.HasOwner;
		
		toOwn.Add<OwnedBy>(owner);
		owner.Add<Owning>(toOwn);
		
		return EEvent.Spawn()
			.Add<OwnershipChangedEvent>()
			.Add<OwnedEntity>(toOwn)
			.Add<NewOwner>(owner);
	}
	
	public static Result<Entity, OwnershipError> RemoveOwner(Entity owned)
	{
		if (!TryGetOwner(owned, out var owner)) return OwnershipError.NoOwner;
		
		owner.Remove<Owning>(owned);
		owned.Remove<OwnedBy>(owner);
		
		return EEvent.Spawn()
			.Add<OwnershipChangedEvent>()
			.Add<OldOwner>(owner)
			.Add<PreviouslyOwned>(owned);
	}
	
	public static Result<(Entity prevOwnerEvent, Entity currentOwnerEvent), OwnershipError> SetOwner(Entity owner, Entity toOwn)
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
		
		return (
			EEvent.Spawn()
				.Add<OwnershipChangedEvent>()
				.Add<OldOwner>(otherOwner)
				.Add<PreviouslyOwned>(toOwn),
			EEvent.Spawn()
				.Add<OwnershipChangedEvent>()
				.Add<NewOwner>(owner)
				.Add<OwnedEntity>(toOwn)
		);
	}
}