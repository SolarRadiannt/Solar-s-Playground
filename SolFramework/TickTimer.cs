namespace SolFramework.Tools;

using Godot;

public struct TimerConfig
{
	public bool? Repeating;
}

public class TickTimer
{
	private static TimerConfig defaultConfig = new TimerConfig
	{
		Repeating = false,
	};

	private bool _justFinished = false;
	private float _duration;
	private float _currentTick = 0f;
	
	public bool Repeating {get; private set;}
	public bool Finished {get; private set;}
	public bool Paused {get; private set;}
	public bool Started {get; private set;}
	
	public float Elapsed => _currentTick;
	public float Countdown => _duration - _currentTick;
	public float Progress => Mathf.Clamp(_currentTick / _duration, 0f, 1f);
	
	public float Duration
	{
		get { return _duration; }
		set
		{
			_duration = Mathf.Max(0, value);
			if (_currentTick >= _duration)
			{
				_currentTick = _duration;
				
				if (Finished) return;
				Finished = true;
				_justFinished = true;
			}
		}
	}
	
	private void _ApplyConfig(TimerConfig config)
	{
		Repeating = config.Repeating.Value;
	}
	
	public TickTimer(float duration, TimerConfig? config)
	{
		_duration = Mathf.Max(0, duration);
		
		if (config.HasValue)
			_ApplyConfig(config.Value);
	}
	public TickTimer(float duration) => _duration = Mathf.Max(0, duration);
	public TickTimer(float duration, bool repeating)
	{
		_duration = Mathf.Max(0, duration);
		Repeating = repeating;
	}
	
	public bool JustFinished()
	{
		if (_justFinished)
		{
			_justFinished = false;
			return true;
		}
		return false;
	}
	
	public bool Reset()
	{
		if (Started)
		{
			Started = false;
			_currentTick = 0;
			
			Finished = false;
			_justFinished = false;
			return true;
		}
		
		return false;
	}
	
	public bool Pause()
	{
		if (Paused) return false;
		
		Paused = true;
		return true;
	}
	
	public bool Resume()
	{
		if (!Paused) return false;
		
		Paused = false;
		return true;
	}
	
	public TickTimer Tick(float delta)
	{
		if (Repeating && Finished) Reset();
		
		if (Paused) return this;
		if (Finished) return this;
		
		if (!Started)
			Started = true;
		
		_currentTick = Mathf.Clamp(_currentTick + delta, 0, _duration);
		if (_currentTick >= _duration)
		{
			Finished = true;
			_justFinished = true;
		}
		
		return this;
	}
	
	public TickTimer Tick(double delta) => Tick((float)delta);
}