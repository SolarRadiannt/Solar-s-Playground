using Godot;

namespace GodotUtilities.Combat;

[GlobalClass]
[Icon("uid://ddekqqgrht5jr")]
public partial class HitboxComponent : Area2D
{
    [Signal] public delegate void DamageDealtEventHandler(HurtboxComponent hurtbox);

    [Export] public DamageType DamageType { get; set; } = DamageType.Physical;

    [Export(PropertyHint.Range, "1, 1000")] public float Damage { get; set; } = 1f;
    [Export(PropertyHint.Range, "0, 1000")] public float KnockbackForce { get; set; }
    
    [Export(PropertyHint.Range, "1, 100")] public float XPMultiplier { get; set; } = 1f;

    private Node2D customDamageSource;
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
        AreaEntered += OnAreaEntered;

        if (!this.TryGetChildOfType(out collision))
            GD.PushError($"[{nameof(HitboxComponent)}] Collision isn't assigned");
    }

    public void SetCustomDamageSource(Node2D source) =>
        customDamageSource = source;

    private void OnAreaEntered(Area2D area)
    {
        if (area is not HurtboxComponent hurtbox) return;

        Vector2 direction = GlobalPosition.DirectionTo(hurtbox.GlobalPosition);
        AttackContext data = new(
            customDamageSource ?? this, 
            Damage, 
            DamageType,
            KnockbackForce * direction,
            XPMultiplier
        );

        hurtbox.ReceiveDamage(data);
        EmitSignalDamageDealt(hurtbox);
    }

}
