namespace HospitalManagementSystem.API.Infrastructure;

public class SimClock : ISimClock
{
    private DateTime _currentTime;
    private readonly object _lock = new object();

    public SimClock()
    {
        _currentTime = DateTime.UtcNow;
    }

    public DateTime GetCurrentTime()
    {
        lock (_lock)
        {
            return _currentTime;
        }
    }

    public DateTime GetCurrentDate()
    {
        lock (_lock)
        {
            return _currentTime.Date;
        }
    }

    public string GetTimeOfDay()
    {
        lock (_lock)
        {
            return _currentTime.ToString("HH:mm:ss");
        }
    }

    public void AdvanceTime(TimeSpan timeSpan)
    {
        lock (_lock)
        {
            _currentTime = _currentTime.Add(timeSpan);
        }
    }
}


