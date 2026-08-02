namespace SolTools.Managers;

using Godot;
using fennecs;

using SolTools.Components;

using SolFramework;
using SolFramework.Components;

using SharpResults.Types;
using System.Linq;
using Root;

public enum ToolError
{
    AlreadyDropped,
    NotATool,
    NoOwner,
    NothingEquipped,
    NotEquipped,
    NotTheOwner,
    AlreadyEquipped,
    AlreadyPickedUp,
    OtherToolEquipped,
}

public static class ToolsManager
{
    // put a dedicated droped tools later
    public static readonly Node2D DroppedToolsContainer = MainGame.Instance;
    private static readonly World world = Core.World;

    public static bool IsTool(Entity entity)
    {
        if (entity.Has<Tool>())
            return true;
        
        GD.PushWarning($"{entity.GetName()} Is not a tool entity!");
        return false;
    }

    public static Entity[] GetOwnedTools(Entity owner) =>
        world.Query()
            .Has<OwnedBy>(owner)
            .Has<Tool>()
            .Compile().ToArray();
    public static bool TryGetEquipped(Entity owner, out Entity tool)
    {
        if (owner.Has<EquippedTool>())
        {
            tool = owner.Ref<EquippedTool>().Value;
            return true;
        }
        tool = default;
        return false;
    }
    public static bool TryGetEquippant(Entity tool, out Entity equippant)
    {
        if (tool.Has<EquippedBy>())
        {
            equippant = tool.Ref<EquippedBy>().Value;
            return true;
        }
        equippant = default;
        return false;
    }
    
    public static bool TryGetOwner(Entity tool, out Entity owner) =>
        world.Query()
            .Has<Owning>(tool)
            .Compile().TryFirst(out owner);
    
    public static Result<Entity, ToolError> Pickup(Entity tool, Entity owner)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        if (TryGetOwner(tool, out var otherOwner))
            if (owner.Equals(otherOwner))
                return ToolError.AlreadyPickedUp;
                // other owner will be managed by systems to
                // let it be given/stolen to them or not
        
        
        return EEvent.Spawn()
            .Add<PickupEvent>()
            .Add(new PickupBy(owner))
            .Add(new PickupTool(tool));
    }

    public static Result<Entity, ToolError> Drop(Entity tool)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        if (!TryGetOwner(tool, out var owner))
            return ToolError.AlreadyDropped;
        
        return EEvent.Spawn()
            .Add<DropEvent>()
            .Add(new DropBy(owner))
            .Add(new DropTool(tool));
    }

	public static Result<Entity, ToolError> Equip(Entity tool, bool swap = false)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        if (!TryGetOwner(tool, out var owner)) return ToolError.NoOwner;
        
        var eevent = EEvent.Spawn()
            .Add<EquippingEvent>()
            .Add(new EquippingBy(owner))
            .Add(new EquippingTool(tool));
        
        if (TryGetEquipped(owner, out var otherTool))
        {
            if (tool.Equals(otherTool)) return ToolError.AlreadyEquipped;
            if (!swap) return ToolError.OtherToolEquipped;
            eevent.Add<SwapEquip>();
        }
        
        return eevent;
    }
    
    public static Result<Entity, ToolError> Unequip(Entity tool, Entity owner)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        if (!TryGetEquipped(owner, out var otherTool)) return ToolError.NothingEquipped;
        if (!tool.Equals(otherTool)) return ToolError.NotEquipped;
        
        return EEvent.Spawn()
            .Add<UnequippingEvent>()
            .Add(new UnequippingBy(owner))
            .Add(new UnequippingTool(tool));
    }
    
    public static Result<Entity, ToolError> Unequip(Entity tool)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        if (!TryGetOwner(tool, out var owner)) return ToolError.NoOwner;
        return Unequip(tool, owner);
    }

    public static Result<Entity, ToolError> OwnerUnequip(Entity owner)
    {
        if (!TryGetEquipped(owner, out var tool)) return ToolError.NothingEquipped;
        return Unequip(tool, owner);
    }
}