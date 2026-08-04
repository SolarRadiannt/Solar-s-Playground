namespace SolFramework;

using System;
using fennecs;
using SolFramework.Components;

public static class Core
{
	public static readonly World World = new();
	
	public static string GetName(Entity entity) => entity.GetName();
	public static void SetName(Entity entity, string name)
	{
		if (entity.Has<Name>())
			entity.Ref<Name>().Value = name;
		else
			entity.Add(new Name(name));
	}
	public static float GetMass(Entity entity) =>
		entity.Has<Mass>() ? entity.Ref<Mass>().Value : 1f;
}