namespace SolItems.Components;
using fennecs;

public struct EquippingEvent;
public struct UnequippingEvent;
public struct PickupEvent;
public struct DropEvent;

public record struct EquippingBy(Entity Value);
public record struct EquippingItem(Entity Value);
public record struct UnequippingBy(Entity Value);
public record struct UnequippingItem(Entity Value);

public record struct DropItem(Entity Value);
public record struct DropBy(Entity Value);

public record struct PickupItem(Entity Value);
public record struct PickupBy(Entity Value);
public struct PickupAlreadyOwned;

public record struct EquippedItem(Entity Value);
public record struct EquippedBy(Entity Value);

public struct ItemType<T>;
public struct SwapEquip;
public struct Item;