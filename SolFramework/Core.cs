namespace SolFramework.Core;
using Godot;
using fennecs;

using SolFramework.Components;

public static class Core
{
	public static World World = new World();
	
	public static string GetName(Entity entity)
	{
		if (entity.Has<Name>())
			return entity.Ref<Name>().Value;
		
		return entity.ToRaw().ToString(); // if no Name get its id
	}
}