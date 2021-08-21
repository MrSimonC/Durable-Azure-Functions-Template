using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace DurableTemplate.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityExample : IEntityExample
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        public void SetName(string name) => Name = name;

        [JsonProperty("events")]
        public List<string> Events { get; set; } = new List<string>();
        public Task<List<string>> GetEvents() => Task.FromResult(Events); // Returning a task means orchestrator will "call" not "signal" this method
        public void SetEvents(List<string> events) => Events = events;
        public void AddEvent(string evnt) => Events.Add(evnt);
        public void RemoveEvent(string evnt) => Events.Remove(evnt);

         [FunctionName(nameof(EntityExample))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<EntityExample>();
    }
}
