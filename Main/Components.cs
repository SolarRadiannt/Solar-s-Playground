using SolFramework;
using SolFramework.Tools;

namespace Root.Components;

public record struct FirearmType<T> : IEcsTag;
public record struct Rifle : IEcsTag;

public record struct Firearm : IEcsTag;
public record struct Pickupable : IEcsTag;
public record struct Firerate(TickTimer Value) : IEcsComponent<TickTimer>;