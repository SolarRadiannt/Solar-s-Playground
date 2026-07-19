using Godot;

namespace GodotUtilities.Movement;

[GlobalClass]
[Icon("uid://bjywwgihunn4o")]
public partial class GravityAdapter : Node, IMovementAdapter
{
    [Signal] public delegate void ReachedMaxFallSpeedEventHandler(float speed);
    [Signal] public delegate void GravityActiveChangedEventHandler(bool active);

    [Export(PropertyHint.Range, "0.1, 10")] public float GravityScale { get; set; } = 1f;
    [Export(PropertyHint.Range, "1, 10")] public float FallMultiplier { get; private set; } = 1f;
    [Export(PropertyHint.Range, "10, 2000")] public float MaxFallSpeed { get; set; } = 500f;

    private static readonly float DefaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private MovementController controller;

    private bool gravityActive = true;
    private bool maxFallSpeedReached;

    public float Gravity => DefaultGravity * GravityScale;
    public bool IsGravityActive => gravityActive;

    public override void _Ready()
    {
        controller = GetParent<MovementController>()
            ?? throw new System.Exception($"{nameof(GravityAdapter)} must be a child of {nameof(MovementController)}");
    }

    public void TickBeforeMoving(float dt)
    {
        if (controller.Body.IsOnFloor() || !gravityActive) return;

        float stackedMultiplier = controller.GetStackedGravityMultiplier();
        float fallMultiplier = controller.IsFalling ? FallMultiplier : 1f;
        float gravity = DefaultGravity * GravityScale * fallMultiplier * stackedMultiplier;

        Vector2 newVelocity = controller.Velocity + controller.Down * gravity * dt;

        float fallSpeed = newVelocity.Dot(controller.Down);
        if (fallSpeed > MaxFallSpeed)
        {
            newVelocity += controller.Up * (fallSpeed - MaxFallSpeed);

            if (!maxFallSpeedReached)
            {
                EmitSignalReachedMaxFallSpeed(fallSpeed);
                maxFallSpeedReached = true;
            }
        }
        else { maxFallSpeedReached = false; }

        controller.SetVelocity(newVelocity);
    }

    public void TickAfterMoving(float dt) { }

    public void SetGravityActive(bool active)
    {
        bool old = gravityActive;
        gravityActive = active;
        if (old != gravityActive) EmitSignalGravityActiveChanged(gravityActive);
    }
}
