using Godot;
using GodotUtilities.UI;

namespace GodotUtilities.InventorySystem;

[GlobalClass]
public partial class PlayerInventoryPanel : UIPanel, IInventoryPanel
{
    [Export] private InventoryUI inventoryUi;

    public override void OnSetup()
    {
        inventoryUi.Attach(InventoryManager.Instance.PlayerInventory);
    }
}
