using Godot;

namespace GodotUtilities.Movement;

[GlobalClass, Icon("uid://bu2m5vmc6c1l4")]
public partial class WallSlideAdapter : Node, IMovementAdapter
{
    [Signal] public delegate void LeftWallEventHandler();
    [Signal] public delegate void TouchedWallEventHandler();
    [Signal] public delegate void WallJumpPerformedEventHandler();

    [ExportGroup("Wall Slide Settings")]
    [Export] private RayCast2D[] rayCasts = [];
    [Export] private float gravityReduction = 0.1f;

    [ExportGroup("Wall Jump Settings")]
    [Export] private float sideJumpWidth = 32f;
    [Export] private float upJumpHeight = 48f;
    [Export] private float wallJumpCooldown = 0.15f;

    [ExportGroup("References")]
    [Export] private JumpAdapter jumpAdapter;
    [Export] private GravityAdapter gravityAdapter;

    public bool IsOnWall { get; private set; }
    public Vector2 WallNormal { get; private set; }

    private MovementController controller;

    private bool wasOnWall;
    private float wallJumpTimer;

    public override void _Ready()
    {
        controller = GetParent<MovementController>() 
            ?? throw new System.Exception($"{nameof(WallSlideAdapter)} must be a child of {nameof(MovementController)}");
    }

    public void TickBeforeMoving(float dt)
    {
        if (wallJumpTimer > 0f) wallJumpTimer -= dt;

        wasOnWall = IsOnWall;

        IsOnWall = TryGetWallNormal(out Vector2 normal);
        WallNormal = normal;

        if (wasOnWall && !IsOnWall) EmitSignalLeftWall();

        if (!wasOnWall && IsOnWall) 
        {
            EmitSignalTouchedWall();

            // Apex hanging might give wrong jump velocity values
            // To avoid this we must consume it once player touches a wall
            jumpAdapter.ConsumeApexHanging();
        }
    }

    public void TickAfterMoving(float dt) { }

    public bool IsOnCooldown() => wallJumpTimer > 0f;

    private bool TryGetWallNormal(out Vector2 normal)
    {
        int colliding = 0;
        Vector2 accumulated = Vector2.Zero;

        foreach (RayCast2D rc in rayCasts)
        {
            if (!rc.IsColliding()) continue;
            accumulated += rc.TargetPosition.Normalized().Rotated(controller.Body.Rotation);
            colliding++;
        }

        normal = accumulated.Normalized();
        return normal != Vector2.Zero;
    }

    public bool TryPerformWallJump()
    {
        if (!jumpAdapter.HasBufferedJump()) return false;

        wallJumpTimer = wallJumpCooldown;

        Vector2 side = controller.Side;
        Vector2 pushDir = -WallNormal.Normalized();

        float pushSide = pushDir.Dot(side);
        float currentUp = controller.Velocity.Dot(controller.Up);
        float force = JumpAdapter.GetJumpVelocity(sideJumpWidth, gravityAdapter.Gravity);

        controller.SetVelocity(controller.Up * currentUp + (side * pushSide * force));
        jumpAdapter.Jump(upJumpHeight);

        EmitSignalWallJumpPerformed();
        return true;
    }

    public void AffectVelocity()
    {
        float fallSpeed = controller.Velocity.Dot(controller.Down);
        float maxSlideSpeed = gravityAdapter.Gravity * gravityReduction;

        if (fallSpeed > maxSlideSpeed)
        {
            Vector2 excess = controller.Up * (fallSpeed - maxSlideSpeed);
            controller.SetVelocity(controller.Velocity + excess);
        }
    }
}
