using Godot;
using GodotUtilities.UI;
using GodotUtilities.Persistence;
using System.Collections.Generic;
using GodotUtilities.InventorySystem.Interaction;

namespace GodotUtilities.InventorySystem;

public interface IInventoryPanel;

[Icon("uid://bf0abwrbo7gg6")]
public partial class InventoryManager : Node, ISaveable
{
    [Signal] public delegate void HotbarUpdatedEventHandler(SlotData data);
    [Signal] public delegate void DropZoneRequestedEventHandler(SlotData data);

    [Save] public InventoryData PlayerInventory { get; private set; }
    [Save] public InventoryData HotbarData { get; private set; }

    public static InventoryManager Instance { get; private set; }
    public InventoryData ExternalData { get; private set; }

    private static readonly Dictionary<StringName, ItemData> ItemCache = [];

    private InteractionHandler interactionHandler;

    private Control dropZone;
    private InventoryUI hotbarUI;

    private SlotData heldItem;
    private ItemPreview itemPreview;
    private Timer stackTimer;

    private PackedScene lootItemScene;

    public string SaveId => "inventory";

    public override void _EnterTree()
    {
        Instance = this;
        AddToGroup(SaveManager.SaveableGroup);
        InitInventories();
    }

    public override void _Ready()
    {
        InitStackTimer();
        InitItemPreview();
        InitDropZone();
        WireSignals();

        lootItemScene = ResourceLoader.Load<PackedScene>(LootItemScenePath);
        interactionHandler = new InteractionHandler(this, stackTimer);
    }

    public override void _Input(InputEvent @event)
    {        
        interactionHandler?.OnInputEvent(@event);
    }

    private void WireSignals()
    {
        if (UIManager.Instance is null) return;
        UIManager.Instance.PanelOpened += OnPanelOpened;
        UIManager.Instance.PanelClosed += OnPanelClosed;
    }

    #region Utilities

    public bool TryStoreItem(SlotData item) =>
        HotbarData.TryStoreItem(item) || PlayerInventory.TryStoreItem(item);

    public static ItemData GetOrLoadItem(StringName id) => 
        FileSystem.GetOrLoad(id, ItemCache, ItemPaths);
    
    public static void ClearItemCache() => ItemCache.Clear();

    public static void UnloadItemData(params StringName[] args)
    {
        foreach (var id in args)
            ItemCache.Remove(id);
    }

    #endregion

    #region Inventory Interact

    private void OnInventoryInteract(InventoryData data, int slotIndex, MouseButton button)
    {
        heldItem = interactionHandler.Interact(heldItem, data, slotIndex, button);
        UpdateItemPreview();
    }

    private void UpdateItemPreview() => itemPreview?.UpdateState(heldItem);

    #endregion

    #region On Inventory Panel Open/Close

    private void OnPanelOpened(StringName panelName)
    {
        var panel = UIManager.Instance.GetPanel(panelName);
        if (panel is not IInventoryPanel) return;

        dropZone.Show();

        // Can access hotbar when inventory is open
        HotbarData.InventoryInteract += OnInventoryInteract;

        if (panel is ExternalInventoryPanel external)
        {
            if (external.Data is null)
            {
                GD.PushError(nameof(InventoryManager), ": ExternalInventoryPanel opened without data assigned. Call SetInventoryData() before opening.");
                return;
            }

            ExternalData = external.Data;
            ExternalData.InventoryInteract += OnInventoryInteract;
        }
    }

    private void OnPanelClosed(StringName panelName)
    {
        if (UIManager.Instance.GetPanel(panelName) is not IInventoryPanel) return;

        dropZone.Hide();
        ClearHeldItem();

        if (ExternalData is not null)
        {
            ExternalData.InventoryInteract -= OnInventoryInteract;
            ExternalData = null;
        }

        // Can not access hotbar when inventory is closed
        HotbarData.InventoryInteract -= OnInventoryInteract;
    }

    #endregion

    #region Hotbar Control

    public int CurrentHotbarPosition { get; private set; }

    public void MoveHotbar(int direction)
    {
        int position = (CurrentHotbarPosition + direction + HotbarData.Capacity) % HotbarData.Capacity;
        SetHotbarPosition(position);
    }
    
    public void SetHotbarPosition(int position)
    {
        if (hotbarUI is null) return;
        if (IsOutOfBounds(position)) return;

        hotbarUI.GetSlotUI(CurrentHotbarPosition)?.OnDeselect();
        CurrentHotbarPosition = position;
        hotbarUI.GetSlotUI(CurrentHotbarPosition)?.OnSelect();

        SlotData slotData = HotbarData.GetSlotData(CurrentHotbarPosition);
        EmitSignalHotbarUpdated(slotData);
    }

    private bool IsOutOfBounds(int position) => position < 0 || position > HotbarData.Capacity - 1;

    #endregion
}
