using Godot;

namespace GodotUtilities;

/// <summary>
/// A lightweight Cooldown Timer. Important Note: Don't mark it as readonly
/// </summary>
/// <param name="defaultDuration"></param>
public struct Cooldown(double defaultDuration)
{
    private readonly double duration = defaultDuration;

    private double currentDuration = defaultDuration;
    private double timer = 0f;

    public readonly bool IsReady => timer <= 0f;
    public readonly double Remaining => Mathf.Max(0f, timer);
    public readonly float Normalized => (float)(1.0 - (timer / currentDuration));

    public void Start(double sec) => timer = currentDuration = sec;
    public void Start() => timer = currentDuration = duration;
    public void Stop() => timer = 0f;

    public void Tick(double dt) => timer = Mathf.Max(0, timer - dt);
}

