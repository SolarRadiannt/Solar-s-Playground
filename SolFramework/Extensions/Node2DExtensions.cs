using Godot;

namespace SolFramework;

public static class Node2DExtensions
{
    public static void LookAtDir(this Node2D node, Vector2 direction) =>  node.LookAt(node.GlobalPosition + direction);
}