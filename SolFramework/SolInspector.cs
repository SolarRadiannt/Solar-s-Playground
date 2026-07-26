namespace SolFramework.Tools;

using Godot;
using fennecs;
using ImGuiGodot;
using ImGuiNET;

using SolFramework.Scheduler;
using SolFramework;

using System;
using System.Reflection;
using System.Collections.Generic;
using SolFramework.Components;
using System.Linq;

public static class SolInspector
{
	private struct ComponentTypeCache
	{
		public Type Type;
		public bool IsTag;
		public FieldInfo[] Fields;
		public PropertyInfo[] Properties;
	}
	
	private static readonly World world = Core.World;
	private static Dictionary<Type, ComponentTypeCache> _memberCache = new();
	private static string _searchFilter = "";
	private static int priority => SPriority.Flush - 10;
	public static void Init()
	{
		Scheduler.RegisterSystem(Process, priority);
		
		GD.Print("SolInspector initialized!");
		
		ImGuiGD.Connect(() =>
		{
			var entities = world.All;
			ImGui.Begin("Sol Inspector");
			
			HandleSearches();
			
			ImGui.Separator();
			
			ProcessEntitiesDisplay(world.All);
			
			ImGui.End();
		});
		
	}
	
	private static void HandleSearches()
	{
		if (ImGui.Button("Clear"))
			_searchFilter = "";
		
		ImGui.SameLine();
		
		ImGui.Text("Search Entities:");
		ImGui.SameLine();
		
		ImGui.SetNextItemWidth(160.0f);
		ImGui.InputText("##Search Entities:", ref _searchFilter, 50, ImGuiInputTextFlags.EscapeClearsAll);
	}
	
	private static void ProcessEntitiesDisplay(Query entities)
	{
		if (ImGui.BeginChild("EntityScrollRegion", new System.Numerics.Vector2(0, 300), ImGuiChildFlags.None))
		{
			foreach (Entity entity in entities)
			{
				string entityName = Core.GetName(entity);
				
				if (!string.IsNullOrWhiteSpace(_searchFilter))
					if (!entityName.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase))
						continue;
				
				ImGui.PushID(entity.GetHashCode());
				DisplayEntity(entity);
				ImGui.PopID();
			}
		}
		ImGui.EndChild(); // --- SCROLLABLE REGION END ---
	}
	
	private static void DisplayEntity(Entity entity)
	{
		if (ImGui.CollapsingHeader(Core.GetName(entity), ImGuiTreeNodeFlags.None))
		{
			ImGui.Indent();
			
			DisplayResources(entity);
			
			ImGui.Unindent();
		}
	}
	
	private static readonly List<(Entity entity, ComponentTypeCache members)>
		entityComponents = new();
	private static readonly List<string>
		entityTags = new();
	
	private static void DisplayResources(Entity entity)
	{
		foreach (var comp in entity.Components)
		{
			var type = comp.Type;
			
			if (!_memberCache.TryGetValue(type, out var members))
			{
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				bool isTag = fields.Length == 0 && properties.Length == 0;
				
				members = new ComponentTypeCache
				{
					Type = type,
					IsTag = isTag,
					Properties = properties,
					Fields = fields,
				};
				
				_memberCache[type] = members;
			}
			
			if (members.IsTag)
				entityTags.Add(type.Name);
			else
				entityComponents.Add((entity, members));
		}
		
		ShowComponents();
		ShowTags();
		
		entityComponents.Clear();
		entityTags.Clear();
	}
	
	private static void ShowComponents()
	{
		if (!ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.None)) return;
		
		ImGui.Indent();
		
		foreach (var data in entityComponents)
			DisplayComponent(data.entity, data.members);
			
		ImGui.Unindent();
	}
	
	private static void ShowTags()
	{
		if (!ImGui.CollapsingHeader("Tags", ImGuiTreeNodeFlags.None)) return;
		
		ImGui.Indent();
		
		foreach (string tagName in entityTags)
			DisplayTag(tagName);
		
		ImGui.Unindent();
	}
	
	private static void DisplayTag(
		string name
	) {
		ImGui.BulletText(name);
	}
	
	private static void DisplayComponent(
		Entity entity,
		ComponentTypeCache members
	) {
		var type = members.Type;
		
		object data = entity.Get(type);
		if (data == null) return;
		
		if (!ImGui.CollapsingHeader(type.Name, ImGuiTreeNodeFlags.None)) return;
		ImGui.Indent();
		
		foreach (var field in members.Fields)
		{
			object value = field.GetValue(data);
			
			ImGui.Text($"{field.Name}: {value}");
		}
		
		foreach (var prop in members.Properties)
		{
			if (prop.GetIndexParameters().Length == 0 && prop.CanRead)
			{
				object value = prop.GetValue(data);
				ImGui.Text($"{prop.Name}: {value}");
			}
		}
		
		ImGui.Unindent();
	}
	
	private static void Process(double delta)
	{
		
	}
}