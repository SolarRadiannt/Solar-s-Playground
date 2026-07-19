using Godot;
using Godot.Collections;

namespace GodotUtilities.Persistence;

[Icon("uid://biumd36d0n5ai")]
public partial class SaveManager : Node
{
    [Signal] public delegate void GameSavedEventHandler(string slotId);
    [Signal] public delegate void GameLoadedEventHandler(string slotId);

    public const string DEFAULT_FOLDER_PATH = "user://saves";
    public const string DEFAULT_SLOT = "slot1";

    public const string PARENT_PATH_ID = "__parent_path__";
    public const string SCENE_PATH_ID = "__scene_path__";

    private const string GAME_SAVED_TEXT = "[color=green]Save Manager: Game Saved ![/color]";
    private const string GAME_LOADED_TEXT = "[color=yellow]Save Manager: Game Loaded ![/color]";

    public static readonly StringName SaveableGroup = "Saveable";

    public static SaveManager Instance { get; private set; }

    private Dictionary saveCache;

    public override void _EnterTree() => Instance = this;

    private string GetSlotPath(string slotId)
        => $"{DEFAULT_FOLDER_PATH}/{slotId}.dat";

    #region SAVE

    public void Save() => Save(DEFAULT_SLOT);

    public void Save(string slotId, string folderPath = DEFAULT_FOLDER_PATH)
    {
        EnsureFolder(folderPath);

        var root = CollectData();

        using var file = FileAccess.Open(GetSlotPath(slotId), FileAccess.ModeFlags.Write);

        if (file == null)
        {
            GD.PrintErr($"Failed to open save file. Error: {FileAccess.GetOpenError()}");
            return;
        }

        file.StoreVar(root);
        EmitSignalGameSaved(slotId);
        GD.PrintRich(GAME_SAVED_TEXT);
    }

    private void EnsureFolder(string path)
    {
        if (DirAccess.DirExistsAbsolute(path))
            return;

        var err = DirAccess.MakeDirRecursiveAbsolute(path);
        if (err != Error.Ok)
            GD.PrintErr($"Failed to create directory: {path}, Error: {err}");
    }

    private Dictionary CollectData()
    {
        var result = new Dictionary();

        foreach (var node in GetTree().GetNodesInGroup(SaveableGroup))
        {
            if (node is not ISaveable saveable)
                continue;

            var key = saveable.SaveId;
            var data = saveable.Serialize();

            if (result.TryGetValue(key, out var existingVar) &&
                existingVar.VariantType == Variant.Type.Dictionary)
            {
                var existing = existingVar.AsGodotDictionary();

                foreach (var kvp in data)
                    existing[kvp.Key] = kvp.Value;
            }
            else
            {
                result[key] = data;
            }
        }

        return result;
    }

    #endregion

    #region LOAD

    public bool Load() => Load(DEFAULT_SLOT);

    public bool Load(string slotId)
    {
        var path = GetSlotPath(slotId);

        if (!FileAccess.FileExists(path))
            return false;

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
            return false;

        var root = file.GetVar().AsGodotDictionary();
        saveCache = root;

        var sceneNodes = BuildSceneNodeMap();

        SpawnMissingNodes(root, sceneNodes);
        DeserializeNodes(root, sceneNodes);

        EmitSignalGameLoaded(slotId);
        GD.PrintRich(GAME_LOADED_TEXT);
        return true;
    }

    private Dictionary<string, Node> BuildSceneNodeMap()
    {
        var nodes = new Dictionary<string, Node>();

        foreach (var node in GetTree().GetNodesInGroup(SaveableGroup))
        {
            if (node is ISaveable saveable)
                nodes[saveable.SaveId] = node;
        }

        return nodes;
    }

    private void SpawnMissingNodes(Dictionary root, Dictionary<string, Node> nodes)
    {
        foreach (var kvp in root)
        {
            var id = kvp.Key.AsString();

            if (nodes.ContainsKey(id))
                continue;

            if (kvp.Value.VariantType != Variant.Type.Dictionary)
                continue;

            var data = kvp.Value.AsGodotDictionary();

            if (!data.TryGetValue(SCENE_PATH_ID, out var sceneVar))
                continue;

            var scenePath = sceneVar.AsString();
            if (string.IsNullOrEmpty(scenePath))
                continue;

            var parentPath = data.TryGetValue(PARENT_PATH_ID, out var pVar)
                ? pVar.AsString()
                : "";

            var prefab = GD.Load<PackedScene>(scenePath);

            if (prefab == null)
            {
                GD.PrintErr($"[SaveManager] Failed to load scene: {scenePath}");
                continue;
            }

            var node = prefab.Instantiate();

            var parent = !string.IsNullOrEmpty(parentPath)
                ? GetNodeOrNull(parentPath)
                : GetTree().CurrentScene;

            parent ??= GetTree().CurrentScene;

            parent.AddChild(node);

            nodes[id] = node;
        }
    }

    private void DeserializeNodes(Dictionary root, Dictionary<string, Node> nodes)
    {
        foreach (var kvp in root)
        {
            var id = kvp.Key.AsString();

            if (!nodes.TryGetValue(id, out var node) || !IsInstanceValid(node))
                continue;

            if (kvp.Value.VariantType != Variant.Type.Dictionary)
                continue;

            var data = kvp.Value.AsGodotDictionary();

            if (node is ISaveable saveable)
                saveable.Deserialize(data);

            if (node.TryGetChildOfType<PropertySaveAdapter>(out var adapter))
            {
                if (adapter is ISaveable adapterSaveable && node != adapter)
                    adapterSaveable.Deserialize(data);
            }
        }
    }

    #endregion

    #region UTILITIES

    public bool SaveExists() => FileAccess.FileExists(GetSlotPath(DEFAULT_SLOT));
    public bool SaveExists(string slotId) => FileAccess.FileExists(GetSlotPath(slotId));

    public void DeleteSave() => DirAccess.RemoveAbsolute(GetSlotPath(DEFAULT_SLOT));
    public void DeleteSave(string slotId) => DirAccess.RemoveAbsolute(GetSlotPath(slotId));

    public bool TryGetData(string key, out Dictionary result)
    {
        result = null;

        if (saveCache == null)
            return false;

        if (!saveCache.TryGetValue(key, out var value))
            return false;

        if (value.VariantType != Variant.Type.Dictionary)
            return false;

        result = value.AsGodotDictionary();
        return result != null;
    }

    public System.Collections.Generic.List<string> GetAvailableSlots()
    {
        var slots = new System.Collections.Generic.List<string>();

        using var dir = DirAccess.Open(DEFAULT_FOLDER_PATH);
        if (dir == null)
            return slots;

        dir.ListDirBegin();

        string fileName = dir.GetNext();

        while (!string.IsNullOrEmpty(fileName))
        {
            if (!dir.CurrentIsDir() && fileName.EndsWith(".dat"))
                slots.Add(fileName.GetBaseName());

            fileName = dir.GetNext();
        }

        return slots;
    }

    #endregion
}