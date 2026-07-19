using System;

namespace GodotUtilities.Persistence;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SaveAttribute(string key = null) : Attribute
{
    public string Key { get; } = key;
}