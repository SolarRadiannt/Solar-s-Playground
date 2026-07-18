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
		
		if (config.Weak.Value && config.Repeating.Value)
			GD.PushWarning("Weak and Repeating config should not coexist! Timer: ", config.Name);
		
		if (config.AutoTick.Value)
			timer.Add<TimerAutoTick>();
		if (config.Weak.Value)
			timer.Add<TimerWeak>();
		if (config.Repeating.Value)
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
	
	public static void SetDuration(Entity timer, float newDuration)
	{
		if (!IsTimer(timer)) return;
		ref var duration = ref timer.Ref<TimerDuration>();
		duration.Value = Math.Max(0, newDuration);
		
		ref var currentTick = ref timer.Ref<TimerTicks>();
		currentTick.Value = Mathf.Max(currentTick.Value, newDuration);
		
		if (currentTick.Value >= newDuration)
			OnFinished(timer);
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
		
		bool finished = timer.Has<TimerFinished>();
		
		if (finished && timer.Has<TimerWeak>())
		{
			GD.PushWarning("Cannot reset ", Core.GetName(timer), " as it is a weak entity!");
			return false;
		}
		if (finished)
			timer.Remove<TimerFinished>();
		if (finished && timer.Has<TimerJustFinished>())
			timer.Remove<TimerJustFinished>();
		
		timer.Ref<TimerTicks>().Value = 0f;
		
		return true;
	}
	
	private static void OnFinished(Entity timer)
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
			OnFinished(timer);
		
		timerTick.Value = newTick;
		return newTick;
	}
	public static float Tick(Entity timer, double by) => Tick(timer, (float)by);
}