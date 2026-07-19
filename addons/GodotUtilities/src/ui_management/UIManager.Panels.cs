using Godot;
using System.Threading.Tasks;

namespace GodotUtilities.UI;

public partial class UIManager
{
    #region Show Panel

    /// <summary>
    /// Shows a HUD or Screen panel. Fire-and-forget overload of <see cref="ShowPanelAsync"/>.
    /// </summary>
    public void ShowPanel(StringName panelName, PanelTransition transition = PanelTransition.Constant) =>
        _ = ShowPanelAsync(panelName, transition);

    /// <summary>
    /// Shows a HUD or Screen panel with an optional transition.
    /// <para>
    /// For Screen panels: if <see cref="UIPanel.IsOverlay"/> is true on the incoming panel,
    /// the current screen is paused but kept visible. Otherwise the current screen is closed first.
    /// </para>
    /// </summary>
    public async Task ShowPanelAsync(StringName panelName, PanelTransition transition = PanelTransition.Constant)
    {
        if (!TryGetPanel(panelName, out var panel))
        {
            GD.PushWarning($"[{nameof(UIManager)}] ShowPanel: panel '{panelName}' not found.");
            return;
        }

        if (panel.Type == UIPanel.PanelType.Popup)
        {
            GD.PushWarning($"[{nameof(UIManager)}] Use ShowPopup() for popup panel '{panelName}'.");
            return;
        }

        if (panel.Type == UIPanel.PanelType.Screen)
        {
            if (stack.Count > 0 && stack.Peek() == panel)
                return;

            if (stack.Count > 0)
            {
                var currentScreen = stack.Peek();
                if (panel.IsOverlay)
                    currentScreen.OnPause();
                else
                    await CloseScreenAsync(stack.Pop(), transition);
            }

            stack.Push(panel);
        }

        await OpenPanelAsync(panel, transition);
    }

    /// <summary>
    /// Shows a Popup panel, waits for <paramref name="delay"/> seconds, then closes it automatically.
    /// Fire-and-forget overload of <see cref="ShowPopupAsync"/>.
    /// </summary>
    public void ShowPopup(StringName panelName, double delay = 2.0) =>
        _ = ShowPopupAsync(panelName, delay);

    /// <inheritdoc cref="ShowPopup"/>
    public async Task ShowPopupAsync(StringName panelName, double delay = 2.0)
    {
        if (!TryGetPanel(panelName, out var popup))
        {
            GD.PushWarning($"[{nameof(UIManager)}] ShowPopup: panel '{panelName}' not found.");
            return;
        }

        if (popup.Type != UIPanel.PanelType.Popup)
        {
            GD.PushWarning($"[{nameof(UIManager)}] ShowPopup: '{panelName}' is not a Popup panel.");
            return;
        }

        await OpenPanelAsync(popup, PanelTransition.Pop);
        await GetTree().Wait(delay);
        await ClosePanelAsync(popup, PanelTransition.Pop);
    }

    #endregion

    #region Hide Panel

    /// <summary>
    /// Hides a HUD panel. Fire-and-forget overload of <see cref="HidePanelAsync"/>.
    /// For Screen panels, use <see cref="GoBack"/> instead.
    /// </summary>
    public void HidePanel(StringName panelName, PanelTransition transition = PanelTransition.Constant) =>
        _ = HidePanelAsync(panelName, transition);

    /// <inheritdoc cref="HidePanel"/>
    public async Task HidePanelAsync(StringName panelName, PanelTransition transition = PanelTransition.Constant)
    {
        if (!TryGetPanel(panelName, out var panel))
        {
            GD.PushWarning($"[{nameof(UIManager)}] HidePanel: panel '{panelName}' not found.");
            return;
        }

        if (panel.Type == UIPanel.PanelType.Screen)
        {
            GD.PushWarning($"[{nameof(UIManager)}] HidePanel: '{panelName}' is a Screen. Use GoBack() instead.");
            return;
        }

        if (!panel.Visible) return;

        await ClosePanelAsync(panel, transition);
    }

    #endregion

    #region Toggle Panel

    /// <summary>
    /// Toggles a HUD panel's visibility. Fire-and-forget overload of <see cref="TogglePanelAsync"/>.
    /// </summary>
    public void TogglePanel(StringName panelName, PanelTransition transition = PanelTransition.Constant) =>
        _ = TogglePanelAsync(panelName, transition);

    /// <inheritdoc cref="TogglePanel"/>
    public async Task TogglePanelAsync(StringName panelName, PanelTransition transition = PanelTransition.Constant)
    {
        if (!TryGetPanel(panelName, out var panel))
        {
            GD.PushWarning($"[{nameof(UIManager)}] TogglePanel: panel '{panelName}' not found.");
            return;
        }

        if (panel.Type == UIPanel.PanelType.Popup)
        {
            GD.PushWarning($"[{nameof(UIManager)}] TogglePanel: use ShowPopup() for popup panel '{panelName}'.");
            return;
        }

        if (!panel.Visible) await ShowPanelAsync(panelName, transition);
        else await HidePanelAsync(panelName, transition);
    }

    #endregion

    #region Go Back

    /// <summary>
    /// Closes the current Screen and returns to the previous one.
    /// Fire-and-forget overload of <see cref="GoBackAsync"/>.
    /// </summary>
    public void GoBack(PanelTransition transition = PanelTransition.Constant) =>
        _ = GoBackAsync(transition);

    /// <summary>
    /// Closes the current Screen and returns to the previous one.
    /// If the closed screen was an overlay, the screen below receives <see cref="UIPanel.OnResume"/>
    /// instead of being re-opened.
    /// </summary>
    public async Task GoBackAsync(PanelTransition transition = PanelTransition.Constant)
    {
        if (stack.Count == 0) return;

        var current = stack.Pop();
        await CloseScreenAsync(current, transition);

        if (stack.Count > 0)
        {
            var prev = stack.Peek();
            if (current.IsOverlay)
                prev.OnResume();
            else
                await OpenPanelAsync(prev, transition);
        }
    }

    #endregion
}