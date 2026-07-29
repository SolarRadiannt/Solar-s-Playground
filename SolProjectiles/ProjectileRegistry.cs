namespace SolProjectiles;
using Godot;
using System.Collections.Generic;

public static class ProjectileRegistry
{
    private static readonly Dictionary<StringName, PackedScene> _scenes = [];

    // Core game registers its defaults on startup
    public static void Register(StringName key, PackedScene scene)
    {
        _scenes[key] = scene;
    }

    public static bool TryGetScene(StringName key, out PackedScene scene)
    {
        return _scenes.TryGetValue(key, out scene);
    }
}