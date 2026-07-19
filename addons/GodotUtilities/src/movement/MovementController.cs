using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GodotUtilities.Movement;

[GlobalClass]
[Icon("uid://bbjg5t7mrw6xe")]
public partial class MovementController : Node
{
    [Signal] public delegate void LandedEventHandler();
    [Signal] public delegate void LeftGroundEventHandler();
    [Signal] public delegate void StartedFallingEventHandler();

    [Export(PropertyHint.Range, "10, 1000")] public float MaxSpeed { get; private set; } = 85f;
    [Export(PropertyHint.Range, "0.1, 100")] public float Mass { get; set; } = 1f;

    [ExportGroup("Control")]
    [Export(PropertyHint.Range, "0, 5")] public float GroundAccelerationTime { get; private set; } = 0.1f;
    [Export(PropertyHint.Range, "0, 5")] public float GroundDecelerationTime { get; private set; } = 0.08f;

    [ExportSubgroup("Air Control")]
    [Export(PropertyHint.Range, "0, 15")] public float AirAccelerationTime { get; private set; } = 0.1f;
    [Export(PropertyHint.Range, "0, 15")] public float AirDecelerationTime { get; private set; } = 0.1f;

    private const float FALL_THRESHOLD = 0.1f;

    public CharacterBody2D Body { get; private set; }

    public bool IsGrounded { get; private set; }
    public bool IsFalling { get; private set; }
    public bool IsRising { get; private set; }

    private readonly List<IMovementAdapter> adapters = new();

    private readonly HashSet<IGravityModifier> gravityMods = new();
    private readonly HashSet<IAccelerationModifier> weightMods = new();

    private Vector2 velocity;

    private bool wasGrounded;
    private bool wasFalling;

    private float GroundAcceleration => GroundAccelerationTime <= 0f ? float.MaxValue : MaxSpeed / GroundAccelerationTime;
    private float GroundDeceleration => GroundDecelerationTime <= 0f ? float.MaxValue : MaxSpeed / GroundDecelerationTime;
    private float AirAcceleration => AirAccelerationTime <= 0f ? float.MaxValue : MaxSpeed / AirAccelerationTime;
    private float AirDeceleration => AirDecelerationTime <= 0f ? float.MaxValue : MaxSpeed / AirDecelerationTime;

    public Vector2 Up => Body.UpDirection;
    public Vector2 Down => -Body.UpDirection;
    public Vector2 Side => Down.Rotated(Mathf.Pi / 2f);
    public Vector2 Velocity => velocity;

    public float FallSpeed => IsFalling ? velocity.Dot(Down) : 0f;
    public float Speed => velocity.Length();
    public float LateralSpeed => Mathf.Abs(velocity.Dot(Side));

    public override void _Ready()
    {
        Body = GetOwner<CharacterBody2D>() ?? throw new Exception(
                $"[{nameof(MovementController)}] Owner must be a CharacterBody2D. " +
                $"Check that '{Name}' is a direct child of your character scene root.");

        foreach (var child in GetChildren().OfType<IMovementAdapter>())
            adapters.Add(child);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        wasGrounded = IsGrounded;
        wasFalling = IsFalling;

        foreach (var adapter in adapters) adapter.TickBeforeMoving(dt);
        
        Commit();
        UpdateState();

        foreach (var adapter in adapters) adapter.TickAfterMoving(dt);
        EmitSignals();
    }

    #region Private API

    private void Commit()
    {
        Body.Velocity = velocity;
        Body.MoveAndSlide();
        velocity = Body.Velocity;
    }
    
    private void UpdateState()
    {
        IsGrounded = Body.IsOnFloor();
        IsFalling = !IsGrounded && velocity.Dot(Down) > FALL_THRESHOLD;
        IsRising = !IsGrounded && velocity.Dot(Up) > FALL_THRESHOLD;
    }

    private void EmitSignals()
    {
        if (!wasGrounded && IsGrounded) EmitSignalLanded();
        if (wasGrounded && !IsGrounded) EmitSignalLeftGround();
        if (!wasFalling && IsFalling) EmitSignalStartedFalling();
    }

    #endregion

    #region Modifiers

    public void RegisterGravityModifier(IGravityModifier modifier) => gravityMods.Add(modifier);
    public void UnregisterGravityModifier(IGravityModifier modifier) => gravityMods.Remove(modifier);

    public void RegisterAccelerationModifier(IAccelerationModifier modifier) => weightMods.Add(modifier);
    public void UnregisterWeightModifier(IAccelerationModifier modifier) => weightMods.Remove(modifier);

    public float GetStackedMotionWeight()
    {
        float stackedWeight = 1f;
        foreach (var mod in weightMods)
            stackedWeight = Mathf.Max(0.05f, stackedWeight * mod.MovementMultiplier);
        return stackedWeight;
    }

    public float GetStackedGravityMultiplier()
    {
        float multiplier = 1f;
        foreach (var mod in gravityMods)
            multiplier = Mathf.Max(0.05f, multiplier * mod.GravityMultiplier);
        return multiplier;
    }

    #endregion

    #region Movement

    private Vector2 ComputeGroundVelocity(Vector2 direction, Vector2 target, float t)
    {
        float velUp = velocity.Dot(Down);
        float velSide = velocity.Dot(Side);

        float targetDown = target.Dot(Down);
        float targetSide = target.Dot(Side);

        if (!Mathf.IsZeroApprox(direction.Dot(Down)))
            velUp = Mathf.MoveToward(velUp, targetDown, t);

        velSide = Mathf.MoveToward(velSide, targetSide, t);
        return Down * velUp + Side * velSide;
    }

    private void ApplyMovement(Vector2 direction, float dt, float speed, float weight)
    {
        if (speed <= 0f)
        {
            GD.PushWarning($"[{nameof(MovementController)}] '{Body.Name}': speed must be greater than zero");
            return;
        }

        Vector2 target = direction.Normalized() * speed;
        float t = weight * GetStackedMotionWeight() * dt;

        velocity = Body.MotionMode switch
        {
            CharacterBody2D.MotionModeEnum.Floating => velocity.MoveToward(target, t),  
            CharacterBody2D.MotionModeEnum.Grounded => ComputeGroundVelocity(direction, target, t),
            _ => Vector2.Zero
        };
    }

    public void MoveWithSpeed(Vector2 direction, float dt, float speed)
    {
        if (direction.IsZeroApprox()) Decelerate(dt);
        else AccelerateWithSpeed(direction, dt, speed);
    }

    public void Move(Vector2 direction, float dt) =>
        MoveWithSpeed(direction, dt, MaxSpeed);
    
    #endregion

    #region Acceleration

    public void AccelerateWithSpeed(Vector2 direction, float dt, float speed)
    {
        float acceleration = IsGrounded ? GroundAcceleration : AirAcceleration;
        ApplyMovement(direction, dt, speed, acceleration);
    }

    public void Accelerate(Vector2 direction, float dt) =>
        AccelerateWithSpeed(direction, dt, MaxSpeed);

    #endregion

    #region Deceleration

    public void Decelerate(float dt)
    {
        float deceleration = IsGrounded ? GroundDeceleration : AirDeceleration;
        ApplyMovement(Vector2.Zero, dt, MaxSpeed, deceleration);
    }

    #endregion

    #region Utilities

    public void SetVelocity(Vector2 value) => velocity = value;
    public void SetVelocityX(float value) => velocity.X = value;
    public void SetVelocityY(float value) => velocity.Y = value;

    public void SetVelocityAlong(Vector2 axis, float value) =>
        velocity = velocity - axis * velocity.Dot(axis) + axis * value;

    public void AddForce(Vector2 force, float dt) => velocity += force * dt / Mass;
    public void AddImpulse(Vector2 impulse) => velocity += impulse / Mass;

    #endregion

}
