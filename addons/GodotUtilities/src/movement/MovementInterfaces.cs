namespace GodotUtilities.Movement;

public interface IMovementAdapter
{
    void TickBeforeMoving(float dt);
    void TickAfterMoving(float dt);
}

public interface IMovementModifier;

public interface IGravityModifier : IMovementModifier
{
    float GravityMultiplier { get; }
}

public interface IAccelerationModifier : IMovementModifier
{
    float MovementMultiplier { get; }
}

