using Godot;

namespace GodotUtilities.InventorySystem.Interaction;

public sealed class InteractionHandler(InventoryManager manager, Timer stackTimer)
{
    private bool isHoldingShift;

    public SlotData Interact(SlotData heldItem, InventoryData data, int index, MouseButton button)
    {
        return (heldItem, button) switch
        {
            (null, MouseButton.Left)  => isHoldingShift ? Transfer(data, index) : Take(data, index),
            (null, MouseButton.Right) => data.Split(index),

            (_, MouseButton.Left) => StackItems(heldItem) ? heldItem : data.Place(heldItem, index),
            (_, MouseButton.Right) => data.PlaceOne(heldItem, index),

            (_, _) => heldItem,
        };
    }

    public void OnInputEvent(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Keycode == Key.Shift)
            isHoldingShift = eventKey.Pressed;
    }

    private SlotData Take(InventoryData data, int slotIndex)
    {
        stackTimer.Start();
        return data.Take(slotIndex);
    }

    private bool StackItems(SlotData receiver)
    {
        if (stackTimer.IsStopped()) return false;

        if (manager.ExternalData is not null)
            InventoryData.StackSimilar(receiver, manager.ExternalData, manager.PlayerInventory, manager.HotbarData);
        else
            InventoryData.StackSimilar(receiver, manager.PlayerInventory, manager.HotbarData);
        return true;
    }

    private SlotData Transfer(InventoryData data, int slotIndex)
    {
        var player = manager.PlayerInventory;
        var hotbar = manager.HotbarData;
        var external = manager.ExternalData;

        if (external is null)
        {
            var target = data == hotbar ? player : hotbar;
            data.TransferItemTo(slotIndex, target);
            return null;
        }

        if (data == player)        TransferWithFallback(data, slotIndex, external, hotbar);
        else if (data == external) TransferWithFallback(data, slotIndex, hotbar, player);
        else                       TransferWithFallback(data, slotIndex, external, player);
        return null;
    }

    private static void TransferWithFallback(InventoryData source, int index, InventoryData primary, InventoryData fallback)
    {
        if (!source.TransferItemTo(index, primary))
            source.TransferItemTo(index, fallback);
    }
}

