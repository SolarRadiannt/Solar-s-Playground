namespace SolFramework.Components;

using fennecs;
using Godot;
using SolFramework.Tools;

[InspectorColor(InspectColor.Indigo)] public record struct Velocity(Vector2 Value) : IEcsComponent<Vector2>;
[InspectorColor(InspectColor.Indigo)] public record struct PushVelocity(Vector2 Value) : IEcsComponent<Vector2>;
[InspectorColor(InspectColor.Indigo)] public record struct ActualVelocity(Vector2 Value) : IEcsComponent<Vector2>;
[InspectorColor(InspectColor.Indigo)] public record struct LastPosition(Vector2 Value) : IEcsComponent<Vector2>;
[InspectorColor(InspectColor.Indigo)] public record struct ActualSpeed(float Value) : IEcsComponent<float>;


[InspectorColor(InspectColor.Indigo)] public record struct Mass(float Value) : IEcsComponent<float>;
[InspectorColor(InspectColor.Gold)] public record struct Name(string Value) : IEcsComponent<string>
{
	public static implicit operator Name(string value) => new(value);
	public static implicit operator string(Name self) => self.Value;
};
[InspectorColor(InspectColor.Gold)] public record struct Variance(float Value) : IEcsComponent<float>;

public record struct PickupDistance(float Value) : IEcsComponent<float>;
public record struct OwnedBy(Entity Target) : IEcsTargetRelation<OwnedBy>;
[InspectorColor(InspectColor.SkyBlue)] public struct Character : IEcsTag;
[InspectorColor(InspectColor.Cyan)] public struct Player : IEcsTag;
public record struct ChildOf : IEcsTag;
public record struct Destroy : IEcsTag;
public record struct Visuals : IEcsTag;

[InspectorColor(InspectColor.Brown)] public struct Grounded : IEcsTag;