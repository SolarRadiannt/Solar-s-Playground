using fennecs;

namespace SolTools.Components;

public struct EquipEvent;
public struct UnequipEvent;
public struct PickupEvent;
public struct DropEvent;

public record struct EquippingBy(Entity Value);
public record struct EquippingTool(Entity Value);
public record struct UnequippingBy(Entity Value);
public record struct UnequippingTool(Entity Value);

public record struct DroppedTool(Entity Value);
public record struct DroppedBy(Entity Value);

public record struct PickedUpTool(Entity Value);
public record struct PickedUpBy(Entity Value);

public struct EquippedBy;
public struct SwapEquip;
public struct Tool;