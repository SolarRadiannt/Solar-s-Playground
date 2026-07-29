namespace SolProjectiles.Components;

using Godot;
using fennecs;

public struct Projectile;
public record struct ProjectileSource(Entity Value);
public record struct ProjectileDamage(float Value);
public record struct ProjectileHitEvent(Entity Value);
public record struct ProjectileCurrentDistance(float Value);
public record struct ProjectileMaxDistance(float Value);
public record struct ProjectileOrigin(Vector2 Value);
public record struct ProjectileScene(PackedScene Value);

public record struct ShootWeapon(Entity Value);
public record struct ShootDirection(Vector2 Value);
public record struct ShootOrigin(Vector2 Value);
public record struct ShootSource(Entity Value);
public record struct ShootProjectileType(StringName Value);
public record struct ShootCollisionMask(uint Value);
public struct ShootEvent;

public record struct HitDataPosition(Vector2 Value);
public record struct HitDataProjectile(BaseProjectile Value);
public record struct HitDataNormal(Vector2 Value);
public record struct HitDataOrigin(Vector2 Value);
public record struct HitDataSource(Entity Value);
public record struct HitDataDistance(float Value);
public record struct HitDataObject(GodotObject Value);
public record struct HitDataRid(Rid Value);
public struct HitEvent;
