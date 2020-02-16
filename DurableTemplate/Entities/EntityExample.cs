using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DurableTemplate.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityExample : IEntityExample
    {
        [JsonProperty("value")]
        public int CurrentValue { get; set; }

        public void Add(int amount) => CurrentValue += amount;

        public void Reset() => CurrentValue = 0;

        public int Get() => CurrentValue;

        [FunctionName(nameof(EntityExample))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<EntityExample>();
    }
}
