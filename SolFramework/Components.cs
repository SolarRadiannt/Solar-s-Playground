namespace SolFramework.Components;
using Godot;
public record struct Velocity(Vector2 Value);
public record struct Name(string Value);

public interface IVelocity
{
	public Vector2 Value {get; set;}
};

public struct ChildOf;
public struct Destroy;
public struct Player;
public struct Grounded;