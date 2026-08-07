namespace SolFramework.Components;

using fennecs;
using Godot;
using SolFramework.Tools;

[InspectorColor(InspectColor.Indigo)] public record struct Velocity(Vector2 Value);
[InspectorColor(InspectColor.Indigo)] public record struct PushVelocity(Vector2 Value);
[InspectorColor(InspectColor.Indigo)] public record struct ActualVelocity(Vector2 Value);
[InspectorColor(InspectColor.Indigo)] public record struct LastPosition(Vector2 Value);
[InspectorColor(InspectColor.Indigo)] public record struct ActualSpeed(float Value);


[InspectorColor(InspectColor.Indigo)] public record struct Mass(float Value);
[InspectorColor(InspectColor.Gold)] public record struct Name(string Value)
{
	public static implicit operator Name(string value) => new(value);
	public static implicit operator string(Name self) => self.Value;
};
[InspectorColor(InspectColor.Gold)] public record struct Variance(float Value);
[InspectorColor(InspectColor.Lime)] public record struct UID(string Value);

public record struct PickupDistance(float Value);
public record struct OwnedBy(Entity Target);
[InspectorColor(InspectColor.SkyBlue)] public struct Character;
[InspectorColor(InspectColor.Cyan)] public struct Player;
public record struct ChildOf;
public record struct Destroy;
public record struct Visuals;


[InspectorColor(InspectColor.Brown)] public struct Grounded;
[InspectorColor(InspectColor.Orange)] public struct Activated;