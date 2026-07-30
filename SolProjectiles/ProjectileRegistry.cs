namespace SolProjectiles;
using Godot;
using System.Collections.Generic;

public static class ProjectileRegistry
{
    private static readonly Dictionary<StringName, ProjectileDefinition> _resources = [];

    // Core game registers its defaults on startup
    public static void Register(StringName key, ProjectileDefinition resource)
    {
        if (_resources.ContainsKey(key))
            GD.PushWarning($"ProjectileRegistry: Overwriting existing key '{key}'");
        
        _resources[key] = resource;
    }

    public static bool TryGetData(StringName key, out ProjectileDefinition resource)
    {
        bool succ = _resources.TryGetValue(key, out resource);
        if (!succ)
            GD.PushWarning($"{key} Is not in the ProjectileRegistry!");
        return succ;
    }

    public static bool TryGetScene(StringName key, out PackedScene scene)
    {
        if (_resources.TryGetValue(key, out var def))
        {
            scene = def.Scene;
            return true;
        }
        scene = null;
        return false;
    }

    public static bool RegisterFromFile(StringName key, string resourcePath)
    {
        var def = ResourceLoader.Load<ProjectileDefinition>(resourcePath);
        if (def == null)
        {
            GD.PrintErr($"Failed to load ProjectileDefinition at: {resourcePath}");
            return false;
        }
        
        Register(key, def);
        return true;
    }

    public static void RegisterAllInFolder(string folderPath, bool recursive = false)
    {
        // 1. Process files in the current folder
        var files = DirAccess.GetFilesAt(folderPath);
        
        if (files != null && files.Length > 0)
        {
            foreach (var file in files)
            {
                if (file.EndsWith(".tres") || file.EndsWith(".res"))
                {
                    var fullPath = $"{folderPath}/{file}";
                    var def = ResourceLoader.Load<ProjectileDefinition>(fullPath);
                    
                    if (def != null)
                    {
                        var key = new StringName(file.Replace(".tres", "").Replace(".res", ""));
                        Register(key, def);
                        GD.Print($"Registered projectile: {key} from {fullPath}");
                    }
                    else
                    {
                        GD.PrintErr($"Failed to load ProjectileDefinition: {fullPath}");
                    }
                }
            }
        }
        else
        {
            GD.Print($"ProjectileRegistry: No files found in {folderPath}");
        }

        // 2. Process subfolders recursively (if enabled)
        if (recursive)
        {
            var dirs = DirAccess.GetDirectoriesAt(folderPath);
            
            // Guard against null (folder missing or no subdirectories)
            if (dirs != null && dirs.Length > 0)
            {
                foreach (var dir in dirs)
                {
                    // Skip hidden folders (optional, but good practice)
                    if (dir.StartsWith("."))
                        continue;
                    
                    // Recurse into the subfolder
                    RegisterAllInFolder($"{folderPath}/{dir}", true);
                }
            }
        }
    }
}