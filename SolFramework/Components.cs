namespace SolFramework.Components;

using fennecs;
using Godot;
using SolFramework.Tools;

[InspectorColor(InspectColor.Violet)] public record struct Node2DHandle(Node2D Value);

[InspectorColor(InspectColor.Indigo)] public record struct Velocity(Vector2 Value);
[InspectorColor(InspectColor.Indigo)] public record struct PushVelocity(Vector2 Value);
[InspectorColor(InspectColor.Indigo)] public record struct ActualVelocity(Vector2 Value);
[InspectorColor(InspectColor.Indigo)] public record struct ActualSpeed(float Value);
[InspectorColor(InspectColor.Indigo)] public record struct LastPosition(Vector2 Value);

[InspectorColor(InspectColor.Indigo)] public record struct Mass(float Value);
[InspectorColor(InspectColor.Gold)] public record struct Name(string Value);
[InspectorColor(InspectColor.Gold)] public record struct Variance(float Value);

[InspectorColor(InspectColor.SkyBlue)] public struct Character;
[InspectorColor(InspectColor.Cyan)] public struct Player;

public struct OwnedBy;
public struct Owning;
public struct ChildOf;
public struct Destroy;

[InspectorColor(InspectColor.Brown)] public struct Grounded;