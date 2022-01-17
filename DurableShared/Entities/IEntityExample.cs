namespace DurableShared.Entities;

public interface IEntityExample
{
    void SetName(string name);
    Task<List<string>> GetEventsAsync();
    void SetEvents(List<string> events);
    void AddEvent(string evnt);
    void RemoveEvent(string evnt);
}
