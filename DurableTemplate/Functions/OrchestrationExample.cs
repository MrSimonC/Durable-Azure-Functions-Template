using DurableShared.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableTemplate.Functions;

public static class OrchestrationExample
{
    [FunctionName(nameof(OrchestrationExample))]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        // param into orchestrator
        OrchestratorInput oi = context.GetInput<OrchestratorInput>();
        // call activity function
        await context.CallActivityAsync<List<string>>(nameof(MyActivityFunction), oi.Url);
        // call durable entity
        List<string> result = await ContactDurableEntity(context);
        // call http
        var request = new DurableHttpRequest(HttpMethod.Post, new Uri(oi.Url), content: "myContent");
        DurableHttpResponse response = await context.CallHttpAsync(request);

        return result;
    }

    [FunctionName(nameof(MyActivityFunction))]
    public static async Task MyActivityFunction(
        [ActivityTrigger] string url,
        ILogger log)
    {
        return;
    }

    private static async Task<List<string>> ContactDurableEntity(
        IDurableOrchestrationContext context)
    {
        var entity = new EntityId(nameof(EntityExample), "myEntityKey");
        IEntityExample entityProxy = context.CreateEntityProxy<IEntityExample>(entity);

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
        string envVar = "MY_ENV_VAR";
        string url = Environment.GetEnvironmentVariable(envVar) ?? throw new ArgumentNullException(envVar);
        OrchestratorInput oi = new() { Url = url };

        string instanceId = await starter.StartNewAsync(nameof(OrchestrationExample), "myInstance", oi.Url);

        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        return starter.CreateCheckStatusResponse(req, instanceId);
    }

    private class OrchestratorInput
    {
        public string Url { get; set; } = string.Empty;
    }
}
