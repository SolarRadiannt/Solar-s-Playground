namespace SolTools.Managers;

using Godot;
using fennecs;

using SolTools.Components;

using SolFramework;
using SolFramework.Components;

using SharpResults.Types;
using SharpResults.Core.Types;

public enum ToolFailReason
{
    NotATool,
    NoOwner,
    NothingEquipped,
    NotEquipped,
    NotTheOwner,
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

    public static bool TryGetEquipped(Entity owner, out Entity tool) =>
        world.Query()
            .Has<EquippedBy>(owner)
            .Compile()
            .TryFirst(out tool);
    
    public static bool Pickup(Entity tool, Entity owner)
    {
        if (!IsTool(tool)) return false;

        EEvent.Spawn()
            .Add<PickupEvent>()
            .Add<PickedUpBy>(owner);

        return true;
    }

    public static bool Drop(Entity tool, Entity owner)
    {
        if (!IsTool(tool)) return false;
        
        EEvent.Spawn()
            .Add<DropEvent>()
            .Add<PickedUpBy>(owner);
        
        return true;
    }

    public static bool TryGetOwner(Entity tool, out Entity owner) =>
        world.Query()
            .Has<Owning>(tool)
            .Compile()
            .TryFirst(out owner);

	public static Result<Unit, ToolFailReason> Equip(Entity tool)
    {
        if (!IsTool(tool)) return ToolFailReason.NotATool;
        if (!TryGetOwner(tool, out var owner)) return ToolFailReason.NoOwner;

        EEvent.Spawn()
            .Add<EquipEvent>()
            .Add<EquippingTool>(tool)
            .Add<Equippant>(owner);

        return Unit.Default;
    }

    public static Result<Unit, ToolFailReason> Unequip(Entity tool)
    {
        if (!IsTool(tool)) return ToolFailReason.NotATool;
        if (!TryGetOwner(tool, out var owner)) return ToolFailReason.NoOwner;
        if (!TryGetEquipped(owner, out var otherTool)) return ToolFailReason.NothingEquipped;
        if (tool != otherTool) return ToolFailReason.NotEquipped;

        return Unit.Default;
    }

    public static Result<Unit, ToolFailReason> OwnerUnequip(Entity owner)
    {
        if (!TryGetEquipped(owner, out var tool)) return ToolFailReason.NothingEquipped;
        if (!TryGetOwner(tool, out var otherOwner)) return ToolFailReason.NoOwner;
        if (owner.ToRaw() != otherOwner.ToRaw()) return ToolFailReason.NotTheOwner;

        EEvent.Spawn()
            .Add<UnequipEvent>();

        return Unit.Default;
    }
}