using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GodotUtilities;

public static class NodeExtensions
{
    public static bool TryGetChildOfType<T>(this Node node, out T result) where T : Node
    {
        foreach (var child in node.GetChildren())
        {
            if (child is T t)
            {
                result = t;
                return true;
            }
        }

        result = null;
        return false;
    }

    public static bool TryGetChildOfTypeRecursive<T>(this Node node, out T result)  where T : Node
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T t)
            {
                result = t;
                return true;
            }

            if (child.TryGetChildOfTypeRecursive(out result))
                return true;
        }

        result = null;
        return false;
    }

    public static T GetChildOfType<T>(this Node node, bool recursive = false) where T : Node
    {
        T result;

        if (recursive) TryGetChildOfTypeRecursive(node, out result);
        else TryGetChildOfType(node, out result);

        return result;
    }

    public static IEnumerable<T> GetChildrenOfType<T>(this Node node) where T : Node =>
        node.GetChildren().OfType<T>();

    public static void DestroyChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
            child.QueueFree();
    }
}
