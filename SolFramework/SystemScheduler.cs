namespace SolFramework.Scheduler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Godot;
	
	public interface ISystem
	{
		public int Priority {get;}
		
		public void Process(double delta);
	}
	public static class SPriority
	{
		public static readonly int Flush = -10_000_000;
		public static readonly int Init = 10_000_000;
		public static readonly int Default = 0;
		public static readonly int Interception = Low;
		public static readonly int Transformation = Lower;
		public static readonly int Applying	= Lowest;
		
		// Generic priorities v
		public static readonly int Lowest	= -10_000;
		public static readonly int Lower	= -1_000;
		public static readonly int Low		= -100;
		public static readonly int High		= 100;
		public static readonly int Higher	= 1_000;
		public static readonly int Highest	= 10_000;
	}
	static class Scheduler
	{
		private static List<(Action<double>, int)> _systems = new();
		public static void RegisterSystem(ISystem system)
		{
			RegisterSystem(system.Process, system.Priority);
		}
		public static void RegisterSystem(Action<double> system, int priority)
		{
			_systems.Add((system, priority));
			_systems = _systems.OrderByDescending(data => data.Item2).ToList();
		}
		public static void ProcessAll(double delta)
		{
			foreach (var data in _systems)
			{
				var system = data.Item1;
				
				try
				{
					system(delta);
				}
				catch (Exception ex)
				{
					GD.PrintErr($"❌ System '{system.Method.DeclaringType.Name}' crashed!");
					GD.PrintErr($"   {ex.Message}");
					GD.PrintErr($"   Stack Trace:");
					GD.PrintErr(ex.StackTrace); // This will contain clickable links
					_systems.Remove(data);
				}
			}
		}
	}
}