using Godot;

namespace GodotUtilities;

public static class CharacterBody2DExtensions
{
    private static readonly float DefaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public static void ApplyGravity(this CharacterBody2D controller, float gravity, float dt, float maxFallSpeed = 1000f)
    {
        if (controller.IsOnFloor())
            return;

        Vector2 floorDirection = -controller.UpDirection;
        controller.Velocity += floorDirection * gravity * dt;

        float speed = controller.Velocity.Dot(floorDirection);

        if (speed > maxFallSpeed)
            controller.Velocity -= floorDirection * (speed - maxFallSpeed);
    }

    public static void ApplyKnockbackFrom(this CharacterBody2D controller, Vector2 sourcePosition, float force)
    {
        Vector2 dir = sourcePosition.DirectionTo(controller.GlobalPosition);
        controller.Velocity += force * dir;
    }

    public static void ApplyKnockback(this CharacterBody2D controller, Vector2 direction, float force)
    {
        Vector2 dir = direction.Normalized();
        controller.Velocity += force * dir;
    }
}

