namespace SolFramework;

using fennecs;
using SolFramework.Components;

public static class Core
{
	public static readonly World World = new();
	
	public static string GetName(Entity entity)
	{
		if (entity.Has<Name>())
			return entity.Ref<Name>().Value;
		
		return entity.ToRaw().ToString(); // if no Name get its id
	}
	public static void SetName(Entity entity, string name)
	{
		if (entity.Has<Name>())
			entity.Ref<Name>().Value = name;
		else
			entity.Add(new Name(name));
	}
}