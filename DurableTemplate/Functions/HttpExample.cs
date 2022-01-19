using DurableShared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace DurableTemplate.Functions;

public partial class HttpExample
{
    ILogger? logger;

    [LoggerMessage(0, LogLevel.Information, "{memberName}() {uniqueKey}: {message}")]
    partial void LogInfo(string message, string uniqueKey, [CallerMemberName] string memberName = "");

    private readonly HttpClient httpClient;
    public HttpExample(IHttpClientFactory httpClientFactory)
    {
        httpClient = httpClientFactory.CreateClient();
    }

    [FunctionName("HttpExample")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log,
        [DurableClient] IDurableEntityClient durableEntityClient)
    {
        logger = log;
        LogInfo("Function Started.", string.Empty);

        // http client json
        string url = "https://mocki.io/v1/d569cadb-8f0b-4271-af06-84368787252d";
        MyClass? response = await httpClient.GetFromJsonAsync<MyClass>(url);

        // durable entity
        var entityId = new EntityId(nameof(EntityExample), "myEntity1");
        EntityStateResponse<EntityExample> stateResponse = await durableEntityClient.ReadEntityStateAsync<EntityExample>(entityId);

        if (!stateResponse.EntityExists)
        {
            LogInfo("Doesn't exist.", entityId.EntityKey);
            await durableEntityClient.SignalEntityAsync<IEntityExample>(entityId, e => e.SetName(response.Name));
            // ...
        }
        else
        {
            LogInfo("Existing entry found.", entityId.EntityKey);
            EntityExample? myEntity = stateResponse.EntityState;
            string entityNameProperty = myEntity.Name;

        }

        return new OkObjectResult($"Hello"); // or BadRequestObjectResult()
    }
}

public class MyClass
{
    public string Name { get; set; } = string.Empty;
}
