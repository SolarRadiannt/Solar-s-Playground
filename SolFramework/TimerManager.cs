namespace SolFramework.TimerManager;

using SolFramework.Core;

using fennecs;
using Godot;
using SolFramework.Components;
using System;

public struct TimerEntity;
public struct TimerWeak;
public struct TimerAutoTick;
public struct TimerRepeating;

public struct TimerFinished;
public struct TimerJustFinished;
public struct TimerPaused;

public record struct TimerDuration(float Value);
public record struct TimerTicks(float Value);

public struct TimerConfig
{
	public string Name;
	public bool? Weak;
	public bool? AutoTick;
	public bool? Repeating;
}

public static class TimerManager
{
	private static readonly World world = Core.World;
	private static void ApplyConfig(Entity timer, TimerConfig config)
	{
		timer.Add(new Name(config.Name));
		
		if (config.Weak.HasValue && config.Repeating.HasValue)
			GD.PushWarning("Weak and Repeating config should not coexist! Timer: ", config.Name);
		
		if (config.AutoTick.HasValue)
			timer.Add<TimerAutoTick>();
		if (config.Weak.HasValue)
			timer.Add<TimerWeak>();
		if (config.Repeating.HasValue)
			timer.Add<TimerRepeating>();
	}
	
	private static bool IsTimer(Entity entity)
	{
		if (entity.Has<TimerEntity>())
			return true;
		
		GD.PushWarning(Core.GetName(entity), "Is not a TimerEntity!");
		return false;
	}
	
	public static Entity Spawn(float duration, TimerConfig? config)
	{
		var timer = world.Spawn()
			.Add<TimerEntity>()
			.Add(new TimerDuration(duration))
			.Add(new TimerTicks(0f));
		
		if (config.HasValue)
			ApplyConfig(timer, config.Value);
		
		return timer;
	}
	public static Entity Spawn(float duration) => Spawn(duration, null);
	
	public static bool Finished(Entity timer)
	{
		if (!IsTimer(timer)) return false;
		
		return timer.Has<TimerFinished>();
	}
	
	public static bool JustFinished(Entity timer)
	{
		if (!IsTimer(timer)) return false;
		if (!timer.Has<TimerJustFinished>()) return false;
		
		timer.Remove<TimerJustFinished>();
		return true;
	}
	
	public static float GetElapsed(Entity timer)
	{
		if (!IsTimer(timer)) return 0f;
		return timer.Ref<TimerTicks>().Value;
	}
	
	public static float GetCountdown(Entity timer)
	{
		if (!IsTimer(timer)) return 0f;
		return timer.Ref<TimerDuration>().Value - timer.Ref<TimerTicks>().Value;
	}
	public static bool IsPaused(Entity timer)
	{
		if (!IsTimer(timer)) return false;
		return timer.Has<TimerPaused>();
	}
	public static bool Pause(Entity timer)
	{
		if (!IsTimer(timer)) return false;
		if (timer.Has<TimerPaused>()) return false;
		
		timer.Add<TimerPaused>();
		return true;
	}
	
	public static bool Resume(Entity timer)
	{
		if (!IsTimer(timer)) return false;
		if (!timer.Has<TimerPaused>()) return false;
		
		timer.Remove<TimerPaused>();
		return true;
	}
	
	public static bool Reset(Entity timer)
	{
		if (!IsTimer(timer)) return false;
		if (!timer.Has<TimerFinished>()) return false;
		if (timer.Has<TimerWeak>())
		{
			GD.PushWarning("Cannot reset ", Core.GetName(timer), " as it is a weak entity!");
			return false;
		}
		
		timer.Remove<TimerFinished>();
		if (timer.Has<TimerJustFinished>())
			timer.Remove<TimerJustFinished>();
		
		return true;
	}
	
	public static float Tick(Entity timer, float delta)
	{
		if (!IsTimer(timer))
			return 0f;
		
		float duration = timer.Ref<TimerDuration>().Value;
		if (timer.Has<TimerFinished>())
			return duration;
		
		ref var timerTick = ref timer.Ref<TimerTicks>();
		
		if (timer.Has<TimerPaused>())
			return timerTick.Value;
		
		float newTick = Mathf.Clamp(timerTick.Value + delta, 0, duration);
		if (newTick >= duration)
		{
			timer.Add<TimerFinished>();
			timer.Add<TimerJustFinished>();
			
			if (timer.Has<TimerWeak>())
				timer.Add<Destroy>();
			else if (timer.Has<TimerRepeating>()) // if timer were weak this wont run if Repeating were enabled
				Callable.From(
				() =>
				{
					if (timer.Alive && timer.Has<TimerRepeating>())
						Reset(timer);
					
				}).CallDeferred(); // deferred to give JustFinished a chance to get called.
			
			return duration;
		}
		
		timerTick.Value = newTick;
		return newTick;
	}
	public static float Tick(Entity timer, double by) => Tick(timer, (float)by);
}