using Godot;
using GodotUtilities.UI;

namespace GodotUtilities.InventorySystem;

[GlobalClass]
public partial class HotbarPanel : UIPanel
{
    [Export] public InventoryUI InventoryUI { get; private set; }

    public override void OnSetup()
    {
        InventoryUI.Attach(InventoryManager.Instance.HotbarData);
        InventoryManager.Instance.InitHotbarUI(InventoryUI);
    }

}
