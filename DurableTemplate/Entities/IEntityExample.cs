using System.Collections.Generic;
using System.Threading.Tasks;

namespace DurableTemplate.Entities
{
    public interface IEntityExample
    {
        void SetName(string name);

        Task<List<string>> GetEvents();
        void SetEvents(List<string> events);
        void AddEvent(string evnt);
        void RemoveEvent(string evnt);
    }
}