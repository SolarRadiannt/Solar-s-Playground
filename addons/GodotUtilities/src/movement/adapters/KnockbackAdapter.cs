using Godot;

namespace GodotUtilities.Movement;

[GlobalClass, Icon("uid://c3fiwxhuhyx50")]
public partial class KnockbackAdapter : Node, IMovementAdapter, IAccelerationModifier
{
    [Signal] public delegate void FinishedEventHandler();

    [Export(PropertyHint.Range, "0.05, 1")] private float duration = 0.25f;
    [Export(PropertyHint.Range, "0, 1")] private float weightScale = 0.1f;
    
    private MovementController controller;
    private float timer;

    public float MovementMultiplier => timer <= 0f ? 1f : weightScale;

    public override void _Ready()
    {
        controller = GetParent<MovementController>() 
            ?? throw new System.Exception($"{nameof(JumpAdapter)} must be a child of {nameof(MovementController)}");
    }

    public void TickBeforeMoving(float dt)
    {
        if (timer <= 0f) return;

        timer -= dt;

        if (timer <= 0f)
            EmitSignalFinished();
    }

    public void TickAfterMoving(float dt) { }

    public void ApplyKnockback(Vector2 impulse) =>
        controller.AddImpulse(impulse);
}
