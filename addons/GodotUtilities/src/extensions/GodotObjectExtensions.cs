using Godot;

namespace GodotUtilities;

public static class GodotObjectExtensions
{
    public static void ConnectOneShot(this GodotObject obj, StringName signal, Callable callable)
    {
        obj.Connect(signal, callable, (uint)GodotObject.ConnectFlags.OneShot);
    }
}

