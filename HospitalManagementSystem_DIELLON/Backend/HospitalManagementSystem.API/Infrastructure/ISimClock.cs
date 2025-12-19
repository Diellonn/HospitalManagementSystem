namespace HospitalManagementSystem.API.Infrastructure;

public interface ISimClock
{
    DateTime GetCurrentTime();
    void AdvanceTime(TimeSpan timeSpan);
    DateTime GetCurrentDate();
    string GetTimeOfDay();
}


