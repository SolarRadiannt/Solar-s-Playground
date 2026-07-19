using Godot;

namespace GodotUtilities.Combat;

[GlobalClass]
[Icon("uid://b72bl8ykpe4fn")]
public partial class HurtboxComponent : Area2D
{
    [Export] private HealthComponent healthComponent;

    private CollisionShape2D collision;
    private bool enabled;

    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            collision?.CallDeferred(CollisionShape2D.PropertyName.Disabled, !enabled);
        }
    }

    public override void _Ready()
    {   
        if (healthComponent is null)
            GD.PushWarning($"[{nameof(HurtboxComponent)}] health component is null");
        
        if (!this.TryGetChildOfType(out collision))
            GD.PushError($"[{nameof(HurtboxComponent)}] Collision isn't assigned");
    }

    public void ReceiveDamage(AttackContext data)
    {
        healthComponent?.TakeDamage(data);
    }
}
