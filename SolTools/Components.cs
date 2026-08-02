using fennecs;

namespace SolTools.Components;

public struct EquippingEvent;
public struct UnequippingEvent;
public struct PickupEvent;
public struct DropEvent;

public record struct EquippingBy(Entity Value);
public record struct EquippingTool(Entity Value);
public record struct UnequippingBy(Entity Value);
public record struct UnequippingTool(Entity Value);

public record struct DropTool(Entity Value);
public record struct DropBy(Entity Value);

public record struct PickupTool(Entity Value);
public record struct PickupBy(Entity Value);
public struct PickupAlreadyOwned;

public record struct EquippedTool(Entity Value);
public record struct EquippedBy(Entity Value);

public struct SwapEquip;
public struct Tool;