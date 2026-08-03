using System.Diagnostics;

public record struct TimeStamper(double? startingTimestamp = null!)
{
	private static double Now() => Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
	
	private double _current = startingTimestamp.HasValue ? startingTimestamp.Value
		: Now();
	
	public void UpdateTimeStamp()
	{
		_current = Now();
	}
	
	public bool CompareNow(double duration) =>
		(Now() - _current) >= duration;
}