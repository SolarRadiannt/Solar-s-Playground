namespace SolFramework.Tools;

using Godot;
using fennecs;
using ImGuiGodot;
using ImGuiNET;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.Components;

using System;
using System.Reflection;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public class InspectorColorAttribute : Attribute
{
	public System.Numerics.Vector4 Color { get; }
	public InspectorColorAttribute(float r, float g, float b, float a = 1.0f)
    {
        Color = new System.Numerics.Vector4(r, g, b, a);
    }
}

public static class SolInspector
{
	private static readonly string TITLE = "Sol ECS Inspector";
	private static readonly int MAX_RECURIONS_DEPTH = 7;
	
	private static readonly float HORIZONTAL_TAGS_SPACE = 16f;
	private struct ComponentTypeCache
	{
		public Type Type;
		public bool IsTag;
		public FieldInfo[] Fields;
		public PropertyInfo[] Properties;
		public System.Numerics.Vector4? CustomColor;
	}
	
	private static readonly World world = Core.World;
	private static Dictionary<Type, ComponentTypeCache> _memberCache = new();
	private static string _searchFilter = "";
	private static int priority => SPriority.Lowest;
	public static void Init()
	{
		Scheduler.RegisterSystem(Process, priority);
		
		ImGuiGD.Connect(() =>
		{
			var entities = world.All;
			ImGui.Begin(TITLE);
			
			HandleSearch("Search Entities:", ref _searchFilter);
			
			ImGui.Separator();
			
			ProcessEntitiesDisplay(world.All);
			
			ImGui.End();
		});
	}
	
	private static bool IsFiltered(string target, string filter)
	{
		if (!string.IsNullOrWhiteSpace(filter))
					if (!target.Contains(filter, StringComparison.OrdinalIgnoreCase))
						return true;
		return false;
	}
	private static void HandleSearch(string title, ref string filter)
	{
		if (ImGui.Button("Clear"))
			filter = "";
		
		ImGui.SameLine();
		
		ImGui.Text(title);
		ImGui.SameLine();
		
		ImGui.SetNextItemWidth(160.0f);
		
		ImGui.InputText($"##{title}", ref filter, 50, ImGuiInputTextFlags.EscapeClearsAll);
	}
	
	private static void ProcessEntitiesDisplay(Query entities)
	{
		if (ImGui.BeginChild("EntityScrollRegion", new System.Numerics.Vector2(0, 0), ImGuiChildFlags.None))
		{
			foreach (Entity entity in entities)
			{
				string entityName = Core.GetName(entity);
				
				if (IsFiltered(entityName, _searchFilter)) continue;
				
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
	
	private static readonly List<ComponentTypeCache>
		entityComponents = new();
	private static readonly List<(string name, System.Numerics.Vector4? color)>
		entityTags = new();
	// Replaced the ECS component approach with a clean dictionary cache for component filters
	private static Dictionary<Entity, string> entitiesComponentFilter = new();
	private static Dictionary<Entity, string> entitiesTagsFilter = new();
	
	private static void DisplayResources(Entity entity)
	{
		foreach (var comp in entity.Components)
		{
			var type = comp.Type;
			var members = GetOrCacheType(type);
			
			if (members.IsTag)
				entityTags.Add((type.Name, members.CustomColor));
			else
				entityComponents.Add(members);
		}
		
		ShowComponents(entity, entityComponents);
		ShowTags(entity, entityTags);
		
		entityComponents.Clear();
		entityTags.Clear();
	}
	
	private static string HandleEntitySearch(string title, Entity entity, Dictionary<Entity, string> filterDict)
	{
		if (!filterDict.TryGetValue(entity, out string filter))
		{
			filter = "";
			filterDict[entity] = filter;
		}
		
		HandleSearch(title, ref filter);
		
		filterDict[entity] = filter;
		return filter;
	}
	private static void ShowComponents(Entity entity, List<ComponentTypeCache> components)
	{
		if (!ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.FramePadding)) return;
		
		ImGui.PushID(entity.GetHashCode());
			string filter = HandleEntitySearch("Search Components:", entity, entitiesComponentFilter);
		ImGui.PopID();
		
		ImGui.Indent();
		
		foreach (var members in components)
		{
			if (IsFiltered(members.Type.Name, filter))
				continue;
			
			DisplayComponent(entity, members);
		}
			
		ImGui.Unindent();
	}
	
	private static void DisplayComponent(Entity entity, ComponentTypeCache members)
	{
		var type = members.Type;
		
		object data = entity.Get(type);
		if (data == null) return;

		bool hasColor = members.CustomColor.HasValue;
		if (hasColor)
			ImGui.PushStyleColor(ImGuiCol.Header, members.CustomColor.Value);
		
		bool isOpen = ImGui.CollapsingHeader(type.Name, ImGuiTreeNodeFlags.None);

		if (hasColor)
			ImGui.PopStyleColor();
		

		if (!isOpen) return;

		ImGui.Indent();
		
		foreach (var field in members.Fields)
		{
			DisplayValue(field.Name, field.GetValue(data));
		}
		
		foreach (var prop in members.Properties)
		{
			if (prop.GetIndexParameters().Length == 0 && prop.CanRead)
			{
				DisplayValue(prop.Name, prop.GetValue(data));
			}
		}
		
		ImGui.Unindent();
	}
	
	private static void ShowTags(Entity entity, List<(string name, System.Numerics.Vector4? color)> tags)
	{
		if (entityTags.Count == 0) return;
		if (!ImGui.CollapsingHeader("Tags", ImGuiTreeNodeFlags.None)) return;
		
		string filter = HandleEntitySearch("Search Tags:", entity, entitiesTagsFilter);
		
		ImGui.Indent();
		
		// Track width to achieve auto-wrapping horizontal layout
		float availableWidth = ImGui.GetContentRegionAvail().X;
		float currentWidth = 0.0f;
		bool firstInRow = true;
		
		foreach (var (tagName, color) in tags)
			if (!IsFiltered(tagName, filter))
				DisplayTag(tagName, color, ref firstInRow, ref currentWidth, availableWidth);
		
		ImGui.Unindent(); // Or just regular ImGui.Unindent();
	}
	
	private static void DisplayTag(string name, System.Numerics.Vector4? color, ref bool firstInRow, ref float currentWidth, float availableWidth)
	{
		var textSize = ImGui.CalcTextSize(name);
		float itemWidth = textSize.X + HORIZONTAL_TAGS_SPACE; // Text width + horizontal padding
		
		// If it exceeds the available window width, wrap to the next line
		if (!firstInRow && (currentWidth + itemWidth) > availableWidth)
		{
			currentWidth = 0.0f;
			firstInRow = true;
		}
		
		if (!firstInRow)
		{
			ImGui.SameLine();
		}
		
		DrawTagBadge(name, color);
		
		currentWidth += itemWidth + 4.0f; // Add a small spacing buffer between tags
		firstInRow = false;
	}
	
	private static void DrawTagBadge(string text, System.Numerics.Vector4? customColor)
	{
		var drawList = ImGui.GetWindowDrawList();
		var cursorPos = ImGui.GetCursorScreenPos();
		var textSize = ImGui.CalcTextSize(text);
		
		System.Numerics.Vector2 padding = new System.Numerics.Vector2(8, 4);
		System.Numerics.Vector2 size = new System.Numerics.Vector2(textSize.X + (padding.X * 2), textSize.Y + (padding.Y * 2));
		
		// Create a soft background color pill (tinted if a custom color attribute is present, otherwise sleek dark slate)
		uint bgColor = customColor.HasValue 
			? ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(customColor.Value.X * 0.4f, customColor.Value.Y * 0.4f, customColor.Value.Z * 0.4f, 0.5f))
			: ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.25f, 0.25f, 0.3f, 0.7f));
			
		uint textColor = customColor.HasValue
			? ImGui.ColorConvertFloat4ToU32(customColor.Value)
			: ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.95f, 0.95f, 0.95f, 1.0f));

		// Draw rounded rectangle background (4.0f is corner roundness)
		drawList.AddRectFilled(
			cursorPos, 
			new System.Numerics.Vector2(cursorPos.X + size.X, cursorPos.Y + size.Y), 
			bgColor, 
			4.0f
		);
		
		// Draw the text inside the pill
		drawList.AddText(
			new System.Numerics.Vector2(cursorPos.X + padding.X, cursorPos.Y + padding.Y), 
			textColor, 
			text
		);
		
		// Advance ImGui's cursor layout space so the next item flows correctly
		ImGui.Dummy(size);
	}
	
	private static void DisplayValue(string name, object value, int depth = 0)
	{
		if (depth > MAX_RECURIONS_DEPTH)
		{
			ImGui.Text($"{name}: [Max Depth Reached]");
			return;
		}
		
		if (value == null)
		{
			ImGui.Text($"{name}: null");
			return;
		}
		
		if (DisplayPrecise(name, value))
			return;
		
		var type = value.GetType();
		
		if (type.IsPrimitive || type.IsEnum
			|| type == typeof(string) || type == typeof(Vector2)
			|| type == typeof(Vector3) || type == typeof(Vector4))
		{
			ImGui.Text($"{name}: {value}");
			return;
		}

		if (ImGui.TreeNode($"{name} ({type.Name})"))
		{
			var members = GetOrCacheType(type);	

			// Display nested fields
			foreach (var field in members.Fields)
				DisplayValue(field.Name, field.GetValue(value), depth + 1);
			
			
			// Display nested properties
			foreach (var prop in members.Properties)
			{
				if (prop.GetIndexParameters().Length == 0 && prop.CanRead)
					try 
					{
						DisplayValue(prop.Name, prop.GetValue(value), depth + 1);
					} 
					catch 
					{
						ImGui.Text($"{prop.Name}: [Error Reading]");
					}
			}
			
			ImGui.TreePop(); // Always pop the tree node when done!
		}
	}
	
	private static bool DisplayPrecise(string name, object value)
	{
		// FORMAT FLOATING POINT NUMBERS HERE
		if (value is float f)
		{
			// "F2" means fixed-point with 2 decimal places (e.g., 3.14)
			// You can change '2' to whatever precision you prefer, or use "0.###" to hide trailing zeros
			ImGui.Text($"{name}: {f:F2}");
			return true;
		}
		if (value is double d)
		{
			ImGui.Text($"{name}: {d:F2}");
			return true;
		}
		if (value is decimal dec)
		{
			ImGui.Text($"{name}: {dec:F2}");
			return true;
		}
		return false;
	}
	
	private static ComponentTypeCache GetOrCacheType(Type type)
	{
		if (!_memberCache.TryGetValue(type, out var members))
		{
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			bool isTag = fields.Length == 0 && properties.Length == 0;
			
			System.Numerics.Vector4? color = null;
			var colorAttr = type.GetCustomAttribute<InspectorColorAttribute>();
			if (colorAttr != null)
			{
				color = colorAttr.Color;
			}
			
			members = new ComponentTypeCache
			{
				Type = type,
				IsTag = isTag,
				Properties = properties,
				Fields = fields,
				CustomColor = color,
			};
			_memberCache[type] = members;
		}
		return members;
	}
	
	// for catching transient event entities later.
	private static void Process(double delta)
	{
		ClearEntitiesFromDicts();
	}

	private static Stream<Destroy>
		destroyingEntities = world.Stream<Destroy>();
	private static void ClearEntitiesFromDicts() =>
		destroyingEntities.For(static
		(in Entity entity, ref Destroy _) =>
		{
			entitiesComponentFilter.Remove(entity);
		});
	
}