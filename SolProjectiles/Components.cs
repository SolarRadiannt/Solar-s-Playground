namespace SolProjectiles.Components;

using Godot;
using fennecs;

public struct Projectile;
public record struct ProjectileHitExcluded;
public record struct ProjectileSource(Entity Value);
public record struct ProjectileDamage(float Value);
public record struct ProjectileHitData(KinematicCollision2D Value);
public record struct ProjectileMaxDistance(float Value);
public record struct ProjectileOrigin(Vector2 Value);
public record struct ProjectileGroupsExclusion(string[] Value);
public record struct ProjectileScene(PackedScene Value);

public record struct ShootProjectile(BaseProjectile Value);
public record struct ShootOrigin(Vector2 Value);
public record struct ShootSource(Entity Value);
public struct ShootEvent;