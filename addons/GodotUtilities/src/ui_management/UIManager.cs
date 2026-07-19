using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace GodotUtilities.UI;

/// <summary>
/// Manages all UI panels: instantiation, layering, transitions, and screen navigation.
/// Add as an autoload. Panels are registered via plugin-generated <c>PanelPaths</c> and <c>HudPanelPaths</c>.
/// </summary>
[Icon("uid://b43xluwyyx2nn")]
public partial class UIManager : Node
{
    /// <summary>Emitted when any panel finishes opening.</summary>
    [Signal] public delegate void PanelOpenedEventHandler(StringName panelName);

    /// <summary>Emitted when any panel finishes closing.</summary>
    [Signal] public delegate void PanelClosedEventHandler(StringName panelName);

    public static UIManager Instance { get; private set; }

    private readonly Dictionary<StringName, UIPanel> panels = new();
    private readonly Stack<UIPanel> stack = new();

    public CanvasLayer HudLayer { get; private set; }
    public CanvasLayer PopupLayer { get; private set; }
    public CanvasLayer ScreenLayer { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;

        HudLayer    = new() { Layer = 1, Name = "HUD" };    AddChild(HudLayer);
        PopupLayer  = new() { Layer = 2, Name = "Popup" };  AddChild(PopupLayer);
        ScreenLayer = new() { Layer = 0, Name = "Screen" }; AddChild(ScreenLayer);
    }

    public override void _Ready()
    {
        foreach (var (id, _) in HudPanelPaths)
            TryGetPanel(id, out _);
    }

    private CanvasLayer ChooseLayer(UIPanel.PanelType type) => type switch
    {
        UIPanel.PanelType.HUD    => HudLayer,
        UIPanel.PanelType.Popup  => PopupLayer,
        UIPanel.PanelType.Screen => ScreenLayer,
        _ => throw new System.ArgumentException($"Invalid Panel Type: '{type}'")
    };

    #region Internal Open / Close

    private async Task OpenPanelAsync(UIPanel panel, PanelTransition transition)
    {
        panel.Show();
        EmitSignalPanelOpened(panel.Id);
        panel.OnOpen();
        await PanelAnimator.Animate(panel, transition, isOpen: true).WaitToFinish();
    }

    private async Task ClosePanelAsync(UIPanel panel, PanelTransition transition)
    {
        await PanelAnimator.Animate(panel, transition, isOpen: false).WaitToFinish();
        panel.OnClose();
        panel.Hide();
        EmitSignalPanelClosed(panel.Id);
    }

    private Task CloseScreenAsync(UIPanel panel, PanelTransition transition) =>
        ClosePanelAsync(panel, transition);

    #endregion

    #region Utilities

    /// <summary>
    /// Returns the panel registered under <paramref name="id"/>, or <c>null</c> if not found.
    /// </summary>
    public UIPanel GetPanel(StringName id) =>
        TryGetPanel(id, out UIPanel panel) ? panel : null;

    /// <summary>
    /// Returns the panel registered under <paramref name="id"/> cast to <typeparamref name="T"/>,
    /// or <c>null</c> if not found or the cast fails.
    /// </summary>
    public T GetPanel<T>(StringName id) where T : UIPanel =>
        TryGetPanel(id, out UIPanel panel) ? panel as T : null;

    public bool TryGetPanel<T>(StringName id, out T panel) where T : UIPanel
    {
        panel = TryGetPanel(id, out UIPanel value) ? value as T : null;
        return panel != null;
    }

    /// <summary>
    /// Retrieves a panel by <paramref name="id"/> from the cache, instantiating it from its registered
    /// scene path if this is the first access. Returns <c>false</c> if the id is unregistered or the scene fails to load.
    /// </summary>
    public bool TryGetPanel(StringName id, out UIPanel panel)
    {
        if (panels.TryGetValue(id, out panel))
            return true;

        if (PanelPaths.TryGetValue(id, out string path))
        {
            var scene = ResourceLoader.Load<PackedScene>(path);
            if (scene is not null)
            {
                panel = scene.Instantiate<UIPanel>();
                ChooseLayer(panel.Type).AddChild(panel);

                panel.Id = id;
                panels[id] = panel;

                if (panel.IsOverlay && panel.Type != UIPanel.PanelType.Screen)
                    GD.PushWarning($"[{nameof(UIManager)}] Panel '{id}' has IsOverlay=true but is not a Screen. IsOverlay has no effect.");

                if (panel.Type != UIPanel.PanelType.HUD)
                    panel.Hide();
                panel.OnSetup();

                UIPanel loadedPanel = panel;
                panel.TreeExiting += () => PanelAnimator.ClearPosition(loadedPanel);
                return true;
            }
            GD.PushWarning($"[{nameof(UIManager)}] failed to load panel for '{id}' at path '{path}'");
        }
        return false;
    }

    #endregion
}

