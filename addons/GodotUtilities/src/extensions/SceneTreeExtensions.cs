using System.Collections.Generic;
using Godot;

namespace GodotUtilities;

public static class SceneTreeExtensions
{
    public static T GetFirstNodeInGroup<T>(this SceneTree sceneTree, StringName group) where T : Node
    {
        var node = sceneTree.GetFirstNodeInGroup(group);
        return node as T;
    }

    public static IEnumerable<T> GetNodesInGroup<T>(this SceneTree sceneTree, StringName group) where T : Node
    {
        foreach (var node in sceneTree.GetNodesInGroup(group))
            yield return node as T;
    }

    public static SignalAwaiter Wait(this SceneTree tree, double seconds) =>
        tree.ToSignal(tree.CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);

    public static SignalAwaiter NextIdle(this SceneTree tree) =>
        tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
}

