namespace SolFramework.TickTimer;

using Godot;

public struct TimerConfig
{
	public bool? Repeating;
}

public struct TickTimer
{
	private static TimerConfig defaultConfig = new TimerConfig
	{
		Repeating = false,
		
	};
	
	private bool _repeating = false;
	private bool _justFinished = false;
	private bool _finished = false;
	private bool _paused = false;
	private bool _started = false;
	
	private float _duration;
	private float _currentTick = 0f;
	
	
	public readonly bool Finished => _finished;
	public readonly bool Paused => _paused;
	public readonly bool Started => _started;
	
	public readonly float Elapsed => _currentTick;
	public readonly float Countdown => _duration - _currentTick;
	public readonly float Progress => Mathf.Clamp(_currentTick / _duration, 0f, 1f);
	
	
	public float Duration
	{
		get { return _duration; }
		set
		{
			_duration = Mathf.Max(0, value);
			if (_currentTick >= _duration)
			{
				_currentTick = _duration;
				
				if (_finished) return;
				_finished = true;
				_justFinished = true;
			}
		}
	}
	
	private void _ApplyConfig(TimerConfig config)
	{
		_repeating = config.Repeating.Value;
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
		_repeating = repeating;
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
		if (_started)
		{
			_started = false;
			_currentTick = 0;
			
			_finished = false;
			_justFinished = false;
			return true;
		}
		
		return false;
	}
	
	public bool Pause()
	{
		if (_paused) return false;
		
		_paused = true;
		return true;
	}
	
	public bool Resume()
	{
		if (!_paused) return false;
		
		_paused = false;
		return true;
	}
	
	public TickTimer Tick(float delta)
	{
		if (_repeating && _finished) Reset();
		
		if (_paused) return this;
		if (_finished) return this;
		
		if (!_started)
			_started = true;
		
		_currentTick = Mathf.Clamp(_currentTick + delta, 0, _duration);
		if (_currentTick >= _duration)
		{
			_finished = true;
			_justFinished = true;
		}
		
		return this;
	}
	
	public TickTimer Tick(double delta) => Tick((float)delta);
}