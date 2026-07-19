using Godot;
using System.Collections.Generic;

namespace GodotUtilities;

public static class FileSystem
{
    #region Public API
    
    /// <summary>
    /// Returns a cached resource by <paramref name="id"/>, loading and caching it on first access.<br/>
    /// Returns <see langword="null"/> and pushes an error if the path exists in <paramref name="pathDict"/>
    /// but the resource fails to load.
    /// </summary>
    /// <param name="id">The key identifying the resource.</param>
    /// <param name="cache">Runtime cache of already-loaded resources.</param>
    /// <param name="pathDict">Map of ids to resource file paths.</param>
    public static T GetOrLoad<T>(StringName id, Dictionary<StringName, T> cache,
        Godot.Collections.Dictionary<StringName, string> pathDict) where T : Resource
    {
        if (cache.TryGetValue(id, out var cached))
            return cached;

        if (pathDict.TryGetValue(id, out string path))
        {
            var resource = GD.Load<T>(path);
            if (resource is not null)
            {
                cache[id] = resource;
                return resource;
            }
            GD.PushError($"ResourceLoaderUtil: failed to load '{id}' at path '{path}'");
        }

        return null;
    }


    public static IEnumerable<T> LoadResourcesInPath<T>(string folderPath, bool recursive = false) where T : Resource
    {
        if (!DirAccess.DirExistsAbsolute(folderPath))
        {
            GD.PushWarning("ResourceLoaderUtil", $"Folder not found: '{folderPath}'");
            yield break;
        }
        foreach (var resource in Walk<T>(folderPath, recursive))
            yield return resource;
    }

    #endregion

    #region Private API

    private static IEnumerable<T> Walk<T>(string folderPath, bool recursive) where T : Resource
    {
        var dir = DirAccess.Open(folderPath);
        if (dir == null)
        {
            GD.PushWarning("ResourceLoaderUtil", $"Could not open folder: '{folderPath}' (Error: {DirAccess.GetOpenError()})");
            yield break;
        }

        dir.ListDirBegin();
        string fileName;
        while ((fileName = dir.GetNext()) != "")
        {
            if (fileName.StartsWith('.'))
                continue;

            string fullPath = $"{folderPath}/{fileName}";

            if (dir.CurrentIsDir())
            {
                if (recursive)
                    foreach (var res in Walk<T>(fullPath, recursive: true))
                        yield return res;
                continue;
            }

            var loaded = TryLoad<T>(fullPath);
            if (loaded != null)
                yield return loaded;
        }
        dir.ListDirEnd();
    }

    private static T TryLoad<T>(string fullPath) where T : Resource
    {
        if (!ResourceLoader.Exists(fullPath))
            return null;

        try { return ResourceLoader.Load<T>(fullPath); }
        catch (System.InvalidCastException) { return null; }
    }

    #endregion
}

