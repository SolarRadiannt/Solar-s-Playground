using Godot;
using GodotUtilities.UI;

namespace GodotUtilities.InventorySystem;

public partial class InventoryManager
{
    private const double STACK_COOLDOWN = 0.15;

    private readonly string HotbarInventoryPath = 
        ProjectSettings.GetSetting("godot_utilities/inventory/hotbar_inventory_path").AsString();

    private readonly string PlayerInventoryPath = 
        ProjectSettings.GetSetting("godot_utilities/inventory/player_inventory_path").AsString();

    private readonly string LootItemScenePath = 
        ProjectSettings.GetSetting("godot_utilities/inventory/loot_item_scene_path").AsString();
    
    private readonly string ItemPreviewScenePath = 
        ProjectSettings.GetSetting("godot_utilities/inventory/item_preview_scene_path").AsString();

    private void InitInventories()
    {
        HotbarData = ResourceLoader.Load<InventoryData>(HotbarInventoryPath);
        HotbarData.InventoryUpdated += d => EmitSignalHotbarUpdated(d.GetSlotData(CurrentHotbarPosition));

        PlayerInventory = ResourceLoader.Load<InventoryData>(PlayerInventoryPath);
        PlayerInventory.InventoryInteract += OnInventoryInteract;
    }

    public void InitHotbarUI(InventoryUI inventoryUI)
    {
        if (hotbarUI is not null) 
            return;

        hotbarUI = inventoryUI;
        SetHotbarPosition(0);
    }
    
    private void InitStackTimer()
    {
        stackTimer = new() { WaitTime = STACK_COOLDOWN, OneShot = true };
        AddChild(stackTimer);
    }

    private void InitItemPreview()
    {
        itemPreview = ResourceLoader.Load<PackedScene>(ItemPreviewScenePath).Instantiate<ItemPreview>();
        UIManager.Instance.HudLayer.AddChild(itemPreview);
        itemPreview.UpdateState(null);
    }

    private void InitDropZone()
    {
        dropZone = new Control() { Name = "inventory_holder", Visible = false };
        dropZone.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        dropZone.GuiInput += OnDropZoneInput;

        UIManager.Instance.ScreenLayer.AddChild(dropZone);
        UIManager.Instance.ScreenLayer.MoveChild(dropZone, 0);
    }

    #region Serialize & Deserialize


    public void OnDeserialize(Godot.Collections.Dictionary data)
    {
        SetHotbarPosition(0);
        UpdateItemPreview();
    }

    #endregion
}

