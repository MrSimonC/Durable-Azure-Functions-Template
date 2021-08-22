using DurableTemplate.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableTemplate.Functions
{
    public static class OrchestrationExample
    {
        [FunctionName(nameof(OrchestrationExample))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // call activity function
            await context.CallActivityAsync<List<string>>(nameof(MyActivityFunction), null);
            // call durable entity
            List<string> result = await ContactDurableEntity(context);
            return result;
        }

        [FunctionName(nameof(MyActivityFunction))]
        public static async Task MyActivityFunction(
            [ActivityTrigger] string name,
            ILogger log)
        {
            return;
        }

        private static async Task<List<string>> ContactDurableEntity(
            IDurableOrchestrationContext context)
        {
            var myEntity = new EntityId(nameof(EntityExample), "myEntityKey");
            IEntityExample? entityProxy = context.CreateEntityProxy<IEntityExample>(myEntity);

            entityProxy.AddEvent("my new event"); // signal (don't wait) as return is void.
            List<string> result = await entityProxy.GetEventsAsync(); // signal (wait & return) as return is a task.
            return result;
        }

        [FunctionName(nameof(HttpStart))]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(OrchestrationExample), null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}