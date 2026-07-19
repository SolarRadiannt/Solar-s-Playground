using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

namespace GodotUtilities.Persistence.Reflection;

public static class ResourceSerializer
{
    private const string RESOURCE_TYPE_ID = "__type__";

    private static readonly Dictionary<string, Type> TypeCache = new();

    private static Type ResolveType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
            return null;

        if (TypeCache.TryGetValue(typeName, out var cached))
            return cached;

        return TypeCache[typeName] = Type.GetType(typeName);
    }

    private static ISerializableResource CreateInstance(Type type)
    {
        if (type == null)
            return null;

        if (type.IsInterface || type.IsAbstract)
        {
            GD.PrintErr($"[Saving] Cannot instantiate '{type.Name}'.");
            return null;
        }

        return Activator.CreateInstance(type) as ISerializableResource;
    }

    private static Type GetSerializedType(Godot.Collections.Dictionary dict)
    {
        if (!dict.TryGetValue(RESOURCE_TYPE_ID, out var typeVar))
            return null;

        return ResolveType(typeVar.AsString());
    }

    public static Godot.Collections.Dictionary SerializeResource(ISerializableResource resource)
    {
        var dict = resource.Serialize();
        dict[RESOURCE_TYPE_ID] = resource.GetType().AssemblyQualifiedName;
        return dict;
    }

    public static ISerializableResource DeserializeResource(Godot.Collections.Dictionary data)
    {
        var type = GetSerializedType(data);

        if (type == null)
        {
            GD.PrintErr("[Saving] Missing or invalid resource type.");
            return null;
        }

        var instance = CreateInstance(type);
        if (instance == null)
            return null;

        instance.Deserialize(data);
        return instance;
    }

    public static void DeserializeMember(SavedMember member, object target, Godot.Collections.Dictionary data)
    {
        if (!data.ContainsKey(member.Key))
            return;

        if (typeof(ISerializableResource).IsAssignableFrom(member.Type))
        {
            HandleSingleResource(member, target, data);
            return;
        }

        if (IsResourceListType(member.Type))
        {
            HandleResourceList(member, target, data);
            return;
        }

        if (IsResourceDictType(member.Type))
        {
            HandleResourceDict(member, target, data);
            return;
        }

        member.SetValue(
            target,
            VariantConverter.FromVariant(data[member.Key], member.Type));
    }

    private static void HandleSingleResource(SavedMember member, object target, Godot.Collections.Dictionary data)
    {
        Variant value = data[member.Key];

        if (value.VariantType == Variant.Type.Nil)
        {
            member.SetValue(target, null);
            return;
        }

        var savedDict = value.AsGodotDictionary();
        var savedType = GetSerializedType(savedDict);

        if (member.GetValue(target) is ISerializableResource existing &&
            (savedType == null || existing.GetType() == savedType))
        {
            existing.Deserialize(savedDict);
            return;
        }

        var instance = CreateInstance(savedType);
        if (instance == null)
            return;

        instance.Deserialize(savedDict);
        member.SetValue(target, instance);
    }

    private static void HandleResourceList(SavedMember member, object target, Godot.Collections.Dictionary data)
    {
        var savedArray = data[member.Key].AsGodotArray();

        bool isGodotArray =
            member.Type.GetGenericTypeDefinition() ==
            typeof(Godot.Collections.Array<>);

        var collection = Activator.CreateInstance(member.Type);

        var addMethod = isGodotArray
            ? member.Type.GetMethod("Add")
            : null;

        foreach (var element in savedArray)
        {
            var resource = DeserializeResource(
                element.AsGodotDictionary());

            if (resource == null)
                continue;

            if (isGodotArray)
            {
                addMethod!.Invoke(collection, new object[] { resource });
            }
            else
            {
                ((IList)collection).Add(resource);
            }
        }

        member.SetValue(target, collection);
    }

    private static void HandleResourceDict(SavedMember member, object target, Godot.Collections.Dictionary data)
    {
        var savedDict = data[member.Key].AsGodotDictionary();

        bool isGodotDict =
            member.Type.GetGenericTypeDefinition() ==
            typeof(Godot.Collections.Dictionary<,>);

        var keyType = member.Type.GetGenericArguments()[0];

        var dictionary = Activator.CreateInstance(member.Type);

        var addMethod = isGodotDict
            ? member.Type.GetMethod("Add")
            : null;

        foreach (var pair in savedDict)
        {
            var key =
                VariantConverter.FromVariant(pair.Key, keyType);

            var resource =
                DeserializeResource(pair.Value.AsGodotDictionary());

            if (resource == null)
                continue;

            if (isGodotDict)
            {
                addMethod!.Invoke(
                    dictionary,
                    new[] { key, (object)resource });
            }
            else
            {
                ((IDictionary)dictionary).Add(key, resource);
            }
        }

        member.SetValue(target, dictionary);
    }

    public static bool IsResourceListType(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericType = type.GetGenericTypeDefinition();

        if (genericType != typeof(List<>) &&
            genericType != typeof(Godot.Collections.Array<>))
            return false;

        return typeof(ISerializableResource)
            .IsAssignableFrom(type.GetGenericArguments()[0]);
    }

    public static bool IsResourceDictType(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericType = type.GetGenericTypeDefinition();

        if (genericType != typeof(Dictionary<,>) &&
            genericType != typeof(Godot.Collections.Dictionary<,>))
            return false;

        return typeof(ISerializableResource)
            .IsAssignableFrom(type.GetGenericArguments()[1]);
    }

    public static bool IsResourceEnumerable(IEnumerable enumerable)
    {
        return enumerable != null &&
               IsResourceListType(enumerable.GetType());
    }

    public static bool IsResourceDict(IDictionary dictionary)
    {
        return dictionary != null &&
               IsResourceDictType(dictionary.GetType());
    }

    public static Godot.Collections.Array SerializeResourceEnumerable(
        IEnumerable enumerable)
    {
        var result = new Godot.Collections.Array();

        foreach (var item in enumerable)
        {
            if (item is ISerializableResource resource)
                result.Add(SerializeResource(resource));
        }

        return result;
    }

    public static Godot.Collections.Dictionary SerializeResourceDict(
        IDictionary dictionary)
    {
        var result = new Godot.Collections.Dictionary();

        foreach (DictionaryEntry pair in dictionary)
        {
            if (pair.Value is not ISerializableResource resource)
                continue;

            result[VariantConverter.ToVariant(pair.Key)] =
                SerializeResource(resource);
        }

        return result;
    }
}