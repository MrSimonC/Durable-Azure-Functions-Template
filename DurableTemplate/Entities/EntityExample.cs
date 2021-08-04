using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DurableTemplate.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityExample : IEntityExample
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        public void SetName(string name) => Name = name;

        [JsonProperty("value")]
        public string Price { get; set; } = string.Empty;
        public void SetPrice(string amount) => Price = amount;

        [JsonProperty("available")]
        public bool Available { get; set; } = true;
        public void SetAvailable(bool available) => Available = available;

        [JsonProperty("onpromotion")]
        public bool OnPromotion { get; set; } = false;
        public void SetOnPromotion(bool available) => OnPromotion = available;

        [JsonProperty("notificationsent")]
        public bool NotificationSent { get; set; } = false;
        public void SetNotificationSent(bool available) => NotificationSent = available;

        public string UniqueId { get; set; } = string.Empty;

        [FunctionName(nameof(EntityExample))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<EntityExample>();
    }
}
