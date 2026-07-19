using Godot;

namespace GodotUtilities.Combat;

public partial class AttackContext(Node2D source, float damage, DamageType type, 
    Vector2 knockback, float xpMultiplier = 1f) : RefCounted
{
    public Node2D Source { get; init; } = source;

    public float EffectiveDamage { get; set; } = damage;
    public float RawDamage { get; init; } = damage;

    public float XPMultiplier { get; init; } = xpMultiplier;

    public DamageType DamageType { get; init; } = type;
    public Vector2 Knockback { get; init; } = knockback;
}
