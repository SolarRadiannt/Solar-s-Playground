namespace SolFramework.Components;

using Godot;
using SolFramework.Tools;


public record struct Velocity(Vector2 Value);
public record struct ActualVelocity(Vector2 Value);
public record struct ActualSpeed(float Value);
public record struct LastPosition(Vector2 Value);

public record struct Name(string Value);

[InspectorColor(0f, 0.7f, 1f)]
public struct Player;
public struct ChildOf;
public struct Destroy;
public struct Grounded;