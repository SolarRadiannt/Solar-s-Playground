using SolFramework;
using SolFramework.Tools;

namespace Root.Components;

[InspectorColor(InspectColor.Gray)] public record struct FirearmType<T> : IEcsTag;
[InspectorColor(InspectColor.Gray)] public record struct Rifle : IEcsTag;

[InspectorColor(InspectColor.Gray)] public record struct Firearm : IEcsTag;
public record struct Pickupable : IEcsTag;
public record struct Firerate(TickTimer Value) : IEcsComponent<TickTimer>;