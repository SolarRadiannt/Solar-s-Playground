using Godot;
using Godot.Collections;

namespace GodotUtilities.Persistence;

/// <summary>
/// Adapts a node to the save system by allowing selected properties
/// to be serialized and restored without modifying the node itself.
///
/// This is especially useful for nodes that:
/// - Do not have a script
/// - Use only built-in properties
/// - Should avoid custom Serialize/Deserialize implementations
///
/// The adapter inspects the parent node and provides a filtered list
/// of valid properties that can be saved via the editor.
/// </summary>
[Tool, GlobalClass, Icon("uid://bbpnakbtrx7wp")]
public partial class PropertySaveAdapter : Node, ISaveable
{
    #region Exported Properties 
    
    [Export] private Array<string> PropertiesToSave { get; set; } = [];
    [Export] public bool DynamicSpawn { get; private set; } = false;

    [Export] private bool UseUniqueId { get => useUniqueId; set { useUniqueId = value; NotifyPropertyListChanged(); } }
    [Export] private string UniqueId { get; set; } = "unique_id";

    #endregion

    #region Cache

    private static readonly System.Collections.Generic.HashSet<string> SkippedProps =
    [
        "",
        "script",
        "__meta__"
    ];

    private readonly System.Collections.Generic.List<string> propertyCache = [];
    private readonly System.Collections.Generic.HashSet<string> validProperties = [];

    #endregion

    #region Backend Fields

    private Node cachedParent;
    private string cachedScenePath;
    private bool useUniqueId;

    #endregion

    #region Getters
    
    private Node ParentNode => cachedParent ??= GetParent();

    public string SaveId => ResolveSaveKey();
    public string ScenePath => ResolveScenePath();

    #endregion

    public override void _Notification(int what)
    {
        if (!Engine.IsEditorHint()) return;

        if (what == NotificationParented) 
            UpdatePropertyList();

        if (what == NotificationPostEnterTree) 
        {
            if (!IsInGroup(SaveManager.SaveableGroup))
                AddToGroup(SaveManager.SaveableGroup);
            UpdatePropertyList();
        }
    }

    public override void _Ready()
    {
        UpdatePropertyList();
    }

    #region Property Resolve

    private string ResolveScenePath()
    {
        if (!DynamicSpawn) return string.Empty;

        if (!string.IsNullOrEmpty(cachedScenePath)) return cachedScenePath;
        cachedScenePath = ParentNode.SceneFilePath;
        return cachedScenePath;
    }

    private string ResolveSaveKey()
    {
        if (UseUniqueId && !string.IsNullOrEmpty(UniqueId)) 
            return $"prop_{UniqueId}";

        if (ParentNode is ISaveable parentSaveable)
            return parentSaveable.SaveId;
        return $"prop_{GetParent().GetPath()}";
    }

    #endregion

    #region Inspector Properties Validation

    private void UpdatePropertyList()
    {
        if (ParentNode == null)
            return;

        propertyCache.Clear();
        validProperties.Clear();

        var props = ParentNode.GetPropertyList();

        foreach (var prop in props)
        {
            if (!prop.TryGetValue("name", out var nameVar))
                continue;

            string name = nameVar.AsString();

            if (SkippedProps.Contains(name))
                continue;

            if (name.StartsWith('/')) continue;
            if (!prop.TryGetValue("type", out var typeVar)) continue;
            if (!prop.TryGetValue("usage", out var usageVar)) continue;

            var usage = (PropertyUsageFlags)usageVar.AsInt32();

            // Only expose properties Godot considers serializable.
            if (!usage.HasFlag(PropertyUsageFlags.Storage)) continue;

            // Skip engine/internal properties.
            if (usage.HasFlag(PropertyUsageFlags.Internal)) continue;

            var type = (Variant.Type)typeVar.AsInt32();

            // Object references require custom handling.
            if (type == Variant.Type.Object)
                continue;

            propertyCache.Add(name);
            validProperties.Add(name);
        }

        NotifyPropertyListChanged();
    }

    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsString();

        if (name == nameof(PropertiesToSave))
        {
            string options = string.Join(",", propertyCache);

            property["hint"] = (int)PropertyHint.TypeString;
            property["hint_string"] =
                $"{(int)Variant.Type.String}/{(int)PropertyHint.Enum}:{options}";
        }

        if (name == nameof(UniqueId))
        {
            property["usage"] = UseUniqueId
                ? (int)PropertyUsageFlags.Default
                : (int)PropertyUsageFlags.None;
        }
    }

    #endregion

    #region Save & Load

    public void OnSerialize(Dictionary data)
    {
        foreach (string prop in PropertiesToSave)
        {
            if (!validProperties.Contains(prop))
                continue;

            data[prop] = ParentNode.Get(prop);
        }
    }

    public void OnDeserialize(Dictionary data)
    {
        foreach (string prop in PropertiesToSave)
        {
            if (!validProperties.Contains(prop))
                continue;

            if (data.TryGetValue(prop, out Variant value))
                ParentNode.Set(prop, value);
        }
    }

    #endregion
}
