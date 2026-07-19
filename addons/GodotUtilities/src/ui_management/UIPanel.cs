using Godot;

namespace GodotUtilities.UI;

/// <summary>
/// Base class for all UI panels. Inherit from this to create HUD elements, screens, or popups.
/// </summary>
[GlobalClass, Icon("uid://b6ggfxy0gygnd")]
public partial class UIPanel : Control
{
    public enum PanelType
    {
        /// <summary>Persistent UI element always visible during gameplay (health bar, minimap).</summary>
        HUD,
        /// <summary>Temporary notification that auto-dismisses after a delay.</summary>
        Popup,
        /// <summary>Full UI screen managed via a navigation stack (main menu, inventory).</summary>
        Screen,
    }

    [Export] public PanelType Type { get; private set; } = PanelType.Screen;

    /// <summary>
    /// If true, this Screen will layer on top of the previous screen without closing it.
    /// The buried screen receives <see cref="OnPause"/> and <see cref="OnResume"/> instead of <see cref="OnClose"/> and <see cref="OnOpen"/>.
    /// Has no effect on non-Screen panels.
    /// </summary>
    [Export] public bool IsOverlay { get; private set; } = false;

    /// <summary>Unique identifier assigned by <see cref="UIManager"/> on instantiation.</summary>
    public StringName Id { get; set; }

    /// <summary>Called once after the panel is instantiated and added to the scene tree.</summary>
    public virtual void OnSetup() { }

    /// <summary>Called every time the panel becomes visible.</summary>
    public virtual void OnOpen() { }

    /// <summary>Called every time the panel is hidden.</summary>
    public virtual void OnClose() { }

    /// <summary>Called when another Screen is pushed on top of this one as an overlay.</summary>
    public virtual void OnPause() { }

    /// <summary>Called when the overlay Screen above this one is closed.</summary>
    public virtual void OnResume() { }
}