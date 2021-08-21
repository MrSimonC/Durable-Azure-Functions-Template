using DurableTemplate.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableTemplate.Functions
{
    public static class OrchestrationExample
    {
        [FunctionName("OrchestrationExample")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            return await context.CallActivityAsync<List<string>>("Get_Entities", null);
        }

        [FunctionName("Get_Entities")]
        public static async Task<List<string>> GetEntitiesAsync(
            [ActivityTrigger] string name,
            ILogger log,
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var egEntity = new EntityId(nameof(EntityExample), "myEntityKey");

            // signal entity
            context.SignalEntity(egEntity, "AddEvent", "my new event");
            // or, with proxy
            EntityExample? entityProxy = context.CreateEntityProxy<EntityExample>(egEntity);
            entityProxy.AddEvent("my new event"); // return is void, so operation is "signaled"
            // get
            return await entityProxy.GetEvents(); // return is a task, so operation is "called"
        }

        [FunctionName("OrchestrationExample_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("OrchestrationExample", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}