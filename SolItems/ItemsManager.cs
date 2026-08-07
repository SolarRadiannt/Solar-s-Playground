namespace SolItems.Managers;

using Godot;
using fennecs;
using Root;

using System.Linq;


using SolFramework;
using SolFramework.Components;

using SharpResults.Types;

using SolItems.Components;
using SolFramework.Managers;

using ItemResult = SharpResults.Types.Result<fennecs.Entity, ItemError>;
public enum ItemError
{
    AlreadyDropped,
    NotAnItem,
    NoOwner,
    NothingEquipped,
    NotEquipped,
    NotTheOwner,
    AlreadyEquipped,
    AlreadyPickedUp,
    OtherToolEquipped,
}

public static class ItemsManager
{
    // put a dedicated droped tools later
    public static readonly Node2D DroppedToolsContainer = MainGame.Instance;
    private static readonly World world = Core.World;

    public static bool IsItem(Entity entity)
    {
        if (entity.Has<Item>())
            return true;
        
        GD.PushWarning($"{entity.GetName()} Is not a tool entity!");
        return false;
    }
    
    public static bool Activate(Entity item)
    {
        if (!IsItem(item)) return false;
        
        return item.TryAdd<Activated>();
    }
    
    public static bool Deactivate(Entity item)
    {
        if (!IsItem(item)) return false;
        
        return item.TryRemove<Activated>();
    }
    
    public static Entity[] GetOwnedItems(Entity owner) =>
        world.Query()
            .Has<OwnedBy>(owner)
            .Has<Item>()
            .Compile().ToArray();
    public static bool TryGetEquipped(Entity owner, out Entity item)
    {
        if (owner.Has<EquippedItem>())
        {
            item = owner.Ref<EquippedItem>().Value;
            return true;
        }
        item = default;
        return false;
    }
    public static bool TryGetEquippant(Entity item, out Entity equippant)
    {
        if (item.Has<EquippedBy>())
        {
            equippant = item.Ref<EquippedBy>().Value;
            return true;
        }
        equippant = default;
        return false;
    }
    
    public static bool TryGetOwner(Entity item, out Entity owner) =>
        OwnershipManager.TryGetOwner(item, out owner);
    
    public static ItemResult Pickup(Entity item, Entity owner)
    {
        if (!IsItem(item)) return ItemError.NotAnItem;
        if (TryGetOwner(item, out var otherOwner))
            if (owner.Equals(otherOwner))
                return ItemError.AlreadyPickedUp;
                // other owner will be managed by systems to
                // let it be given/stolen to them or not
        
        
        return EEvent.Spawn()
            .Add<PickupEvent>()
            .Add(new PickupBy(owner))
            .Add(new PickupItem(item));
    }

    public static ItemResult Drop(Entity item, Entity owner)
    {
        if (!IsItem(item)) return ItemError.NotAnItem;
        if (TryGetOwner(item, out var currentOwner) && !currentOwner.Equals(owner))
            return ItemError.NotTheOwner;
        
        return EEvent.Spawn()
            .Add<DropEvent>()
            .Add(new DropBy(owner))
            .Add(new DropItem(item));
    }

    public static ItemResult Drop(Entity item)
    {
        if (!IsItem(item)) return ItemError.NotAnItem;
        if (!TryGetOwner(item, out var owner))
            return ItemError.AlreadyDropped;
        
        return Drop(item, owner);
    }

	public static ItemResult Equip(Entity item, bool swap = false)
    {
        if (!IsItem(item)) return ItemError.NotAnItem;
        if (!TryGetOwner(item, out var owner)) return ItemError.NoOwner;
        
        var eevent = EEvent.Spawn()
            .Add<EquippingEvent>()
            .Add(new EquippingBy(owner))
            .Add(new EquippingItem(item));
        
        if (TryGetEquipped(owner, out var otherTool))
        {
            if (item.Equals(otherTool)) return ItemError.AlreadyEquipped;
            if (!swap) return ItemError.OtherToolEquipped;
            eevent.Add<SwapEquip>();
        }
        
        return eevent;
    }
    
    public static ItemResult Unequip(Entity item, Entity owner)
    {
        if (!IsItem(item)) return ItemError.NotAnItem;
        if (!TryGetEquipped(owner, out var otherTool)) return ItemError.NothingEquipped;
        if (!item.Equals(otherTool)) return ItemError.NotEquipped;
        
        return EEvent.Spawn()
            .Add<UnequippingEvent>()
            .Add(new UnequippingBy(owner))
            .Add(new UnequippingItem(item));
    }
    
    public static ItemResult Unequip(Entity item)
    {
        if (!IsItem(item)) return ItemError.NotAnItem;
        if (!TryGetOwner(item, out var owner)) return ItemError.NoOwner;
        return Unequip(item, owner);
    }

    public static ItemResult OwnerUnequip(Entity owner)
    {
        if (!TryGetEquipped(owner, out var tool)) return ItemError.NothingEquipped;
        return Unequip(tool, owner);
    }
}