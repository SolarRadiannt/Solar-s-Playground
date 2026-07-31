namespace SolTools.Managers;

using Godot;
using fennecs;

using SolTools.Components;

using SolFramework;
using SolFramework.Components;

using SharpResults.Types;
using SharpResults.Core.Types;
using System.Linq;


public enum ToolError
{
    NotATool,
    NoOwner,
    NothingEquipped,
    NotEquipped,
    NotTheOwner,
    AlreadyEquipped,
}

public static class ToolsManager
{
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

    public static bool TryGetEquipped(Entity owner, out Entity tool) =>
        world.Query()
            .Has<EquippedBy>(owner)
            .Has<Tool>()
            .Compile().TryFirst(out tool);
    
    public static bool TryGetOwner(Entity tool, out Entity owner) =>
        world.Query()
            .Has<Owning>(tool)
            .Has<Tool>()
            .Compile().TryFirst(out owner);
    
    public static Result<Entity, ToolError> Pickup(Entity tool, Entity owner)
    {
        if (!IsTool(tool)) return ToolError.NotATool;

        return EEvent.Spawn()
            .Add<PickupEvent>()
            .Add<PickedUpBy>(owner)
            .Add<PickupTool>(tool);
    }

    public static Result<Entity, ToolError> Drop(Entity tool, Entity owner)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        
        return EEvent.Spawn()
            .Add<DropEvent>()
            .Add<PickedUpBy>(owner)
            .Add<DropTool>(tool);
    }

	public static Result<Entity, ToolError> Equip(Entity tool, bool swap = false)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        if (!TryGetOwner(tool, out var owner)) return ToolError.NoOwner;
        if (TryGetEquipped(owner, out var otherTool))
            if (tool.ToRaw() == otherTool.ToRaw())
                return ToolError.AlreadyEquipped;
        
        var eevent = EEvent.Spawn()
            .Add<EquipEvent>()
            .Add<EquippingTool>(tool)
            .Add<Equippant>(owner);
        
        if (swap)
            eevent.Add<SwapEquip>();
        
        return eevent;
    }

    public static Result<Entity, ToolError> Unequip(Entity tool)
    {
        if (!IsTool(tool)) return ToolError.NotATool;
        if (!TryGetOwner(tool, out var owner)) return ToolError.NoOwner;
        if (!TryGetEquipped(owner, out var otherTool)) return ToolError.NothingEquipped;
        if (tool != otherTool) return ToolError.NotEquipped;
        
        return EEvent.Spawn()
            .Add<UnequipEvent>()
            .Add<Unequippant>(owner)
            .Add<UnequippedTool>(tool);
    }

    public static Result<Entity, ToolError> OwnerUnequip(Entity owner)
    {
        if (!TryGetEquipped(owner, out var tool)) return ToolError.NothingEquipped;
        if (!TryGetOwner(tool, out var otherOwner)) return ToolError.NoOwner;
        if (owner.ToRaw() != otherOwner.ToRaw()) return ToolError.NotTheOwner;

        return EEvent.Spawn()
            .Add<UnequipEvent>()
            .Add<Unequippant>(owner)
            .Add<UnequippedTool>(tool);
    }
}