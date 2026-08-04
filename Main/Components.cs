using SolFramework;
using SolFramework.Tools;

namespace Root.Components;

[InspectorColor(InspectColor.Gray)] public record struct FirearmType<T>;
[InspectorColor(InspectColor.Gray)] public record struct Rifle;

[InspectorColor(InspectColor.Gray)] public record struct Firearm;
public record struct Pickupable;
public record struct Firerate(TickTimer Value);