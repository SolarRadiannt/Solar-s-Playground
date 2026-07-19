using Godot;
using System;

namespace GodotUtilities.Movement;

[GlobalClass]
[Icon("uid://be3perrv6bhvf")]
public partial class JumpAdapter : Node, IMovementAdapter, IGravityModifier
{
    [Signal] public delegate void JumpedEventHandler();
    [Signal] public delegate void ApexReachedEventHandler();
    [Signal] public delegate void CoyoteJumpPerformedEventHandler();
    [Signal] public delegate void AerialJumpsChangedEventHandler(int remaining);

    [Export] private GravityAdapter gravityAdapter;

    [ExportGroup("Jump Settings")]
    [Export(PropertyHint.Range, "0, 4")] public int MaxAerialJumps { get; set; } = 0;
    [Export(PropertyHint.Range, "10, 200")] public float JumpHeight { get; private set; } = 48f;
    [Export(PropertyHint.Range, "1, 200")] public float MinJumpHeight { get; private set; } = 48f;

    [ExportGroup("Timing")]
    [Export(PropertyHint.Range, "0, 1")] public float CoyoteTime { get; private set; } = 0.12f;
    [Export(PropertyHint.Range, "0, 1")] public float JumpBufferTime { get; private set; } = 0.12f;

    [ExportGroup("Apex Hanging")]
    [Export(PropertyHint.Range, "0, 0.5")] public float ApexDuration { get; private set; } = 0.08f;
    [Export(PropertyHint.Range, "1, 80")] public float ApexThreshold { get; private set; } = 30f;
    [Export(PropertyHint.Range, "0.05, 0.9")] public float ApexGravityScale { get; private set; } = 0.3f;

    public int AerialJumpsRemaining { get; private set; }

    private MovementController controller;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float apexTimer;

    private bool apexConsumed;
    
    // Prevents coyote from being acquired when we leave the ground due to jumping
    // rather than walking off an edge
    private bool coyoteEligible;

    public float GravityMultiplier => apexTimer > 0f ? ApexGravityScale : 1f;

    public bool IsOutOfAerialJumps => AerialJumpsRemaining == 0;
    public bool IsAtMaxAerialJumps => AerialJumpsRemaining == MaxAerialJumps;
    public bool IsAtApex => apexTimer > 0f;

    public override void _Ready()
    {
        controller = GetParent<MovementController>() 
            ?? throw new Exception($"{nameof(JumpAdapter)} must be a child of {nameof(MovementController)}");
        
        controller.RegisterGravityModifier(this);

        controller.Landed += OnControllerLanded;
        controller.StartedFalling += OnControllerFalling;
    }

    #region Tick

    public void TickBeforeMoving(float dt)
    {
        TickTimers(dt);
        HandleApex();
    }

    public void TickAfterMoving(float dt) { }

    private void TickTimers(float dt)
    {
        if (apexTimer > 0f) apexTimer -= dt;
        if (coyoteTimer > 0f) coyoteTimer -= dt;
        if (jumpBufferTimer > 0f) jumpBufferTimer -= dt;
    }

    #endregion

    #region Signal Connection

    private void OnControllerLanded()
    {
        SetAerialJumps(MaxAerialJumps);
        apexConsumed = false;
        coyoteEligible = true;
    }

    private void OnControllerFalling()
    {
        if (coyoteEligible)
            AcquireCoyote();
    }

    #endregion

    #region Coyote Jump & Jump Buffering

    public bool HasCoyote() => coyoteTimer > 0f;
    public void ConsumeCoyote() => coyoteTimer = 0f;
    public void AcquireCoyote() => coyoteTimer = CoyoteTime;

    public void BufferJump() => jumpBufferTimer = JumpBufferTime;
    public bool HasBufferedJump() => jumpBufferTimer > 0f;
    public void ConsumeBufferedJump() => jumpBufferTimer = 0f;

    #endregion

    #region Jump

    public void AddAerialJump() => SetAerialJumps(AerialJumpsRemaining + 1);
    public void ConsumeAerialJump() => SetAerialJumps(AerialJumpsRemaining - 1);

    public bool CanGroundJump() => controller.IsGrounded || HasCoyote();
    public bool CanAerialJump() => !CanGroundJump() && AerialJumpsRemaining > 0;

    public bool CanJump() => HasBufferedJump() && (CanGroundJump() || CanAerialJump());

    public void Jump() => Jump(JumpHeight);

    public bool TryJump()
    {
        if (CanJump()) { Jump(); return true; }
        return false;
    }

    public void Jump(float height)
    {
        float verticalSpeed = controller.Velocity.Dot(controller.Up);
        float targetVelocity = GetJumpVelocity(height, gravityAdapter.Gravity);
        float impulse = targetVelocity - verticalSpeed;

        controller.AddImpulse(controller.Up * impulse);

        bool hadCoyote = HasCoyote();
        bool usedGroundJump = controller.IsGrounded || hadCoyote;
        coyoteEligible = false; 

        ConsumeCoyote();
        ConsumeBufferedJump();

        if (!usedGroundJump) ConsumeAerialJump();
        if (hadCoyote) EmitSignalCoyoteJumpPerformed();

        EmitSignalJumped();
    }

    public static float GetJumpVelocity(float height, float gravity) =>
        Mathf.Sqrt(2f * height * gravity);

    private void SetAerialJumps(int count)
    {
        int oldValue = AerialJumpsRemaining;
        AerialJumpsRemaining = Mathf.Clamp(count, 0, MaxAerialJumps);

        if (oldValue != AerialJumpsRemaining)
            EmitSignalAerialJumpsChanged(AerialJumpsRemaining);
    }

    #endregion

    #region Apex Hanging

    public void ConsumeApexHanging() => apexConsumed = true;

    private void HandleApex()
    {
        if (controller.IsGrounded || ApexDuration == 0f) return;

        float vUp = controller.Velocity.Dot(controller.Up);
        bool isNearApex = vUp > 0f && vUp < ApexThreshold;

        if (!apexConsumed && isNearApex && controller.IsRising)
        {
            apexTimer = ApexDuration;
            apexConsumed = true;
            EmitSignalApexReached();
        }
    }

    #endregion

    #region Jump Release

    public void OnJumpReleased()
    {
        if (controller.IsFalling) return;

        float upSpeed = controller.Velocity.Dot(controller.Up);
        float minSpeed = GetJumpVelocity(MinJumpHeight, gravityAdapter.Gravity);

        if (upSpeed > minSpeed) 
            controller.SetVelocityAlong(controller.Up, minSpeed);
    }

    #endregion
}
