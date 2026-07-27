namespace SolFramework;
using fennecs;
using System.Diagnostics.CodeAnalysis;
public static class EntityExtensions
{
	public static bool TryRef<T>(this Entity entity, out T component)
	{
		if (entity.Has<T>())
		{
			component = entity.Ref<T>();
			return true;
		}
		component = default;
		return false;
	}

	public static bool TryAdd<T>(this Entity entity, T component)
	{
		if (entity.Has<T>()) return false;
		entity.Add(component);
		return true;
	}
	public static bool TryAdd<T>(this Entity entity)
		where T : struct
	{
		if (entity.Has<T>()) return false;
		entity.Add<T>();

		return true;
	}

	public static void Set<T>(this Entity entity, T component)
	{
		if (entity.Has<T>())
			entity.Ref<T>() = component;
		else
			entity.Add(component);
	}

	// ============ UNIVERSAL (Structs & Classes) ============

	public static bool TryRemove<T>(this Entity entity) // ✅ No constraint!
	{
		if (!entity.Has<T>()) return false;
		entity.Remove<T>();
		return true;
	}

	// ============ Multi-Checks ============

	// Works for any T1, T2 (struct, class, or record)
	public static bool HasAll<T1, T2>(this Entity entity) 
		=> entity.Has<T1>() && entity.Has<T2>();

	public static bool HasAll<T1, T2, T3>(this Entity entity) 
		=> entity.Has<T1>() && entity.Has<T2>() && entity.Has<T3>();

	public static bool HasAny<T1, T2>(this Entity entity) 
		=> entity.Has<T1>() || entity.Has<T2>();

	public static bool HasAny<T1, T2, T3>(this Entity entity) 
		=> entity.Has<T1>() || entity.Has<T2>() || entity.Has<T3>();
}