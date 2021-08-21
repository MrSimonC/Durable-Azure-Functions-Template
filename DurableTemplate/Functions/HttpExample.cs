using DurableTemplate.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace DurableTemplate.Functions
{
    public class HttpExample
    {
        private readonly HttpClient httpClient;
        public HttpExample(IHttpClientFactory httpClientFactory) => httpClient = httpClientFactory.CreateClient();

        [FunctionName("HttpExample")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [DurableClient] IDurableEntityClient durableEntityClient,
            IDurableEntityContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            MyClass? response = await httpClient.GetFromJsonAsync<MyClass>("https://github.com");
            if (string.IsNullOrEmpty(response?.Name))
            {
                return new BadRequestObjectResult($"Name was empty");
            }

            var entityId = new EntityId(nameof(EntityExample), "myEntity1");
            EntityStateResponse<EntityExample> stateResponse = await durableEntityClient.ReadEntityStateAsync<EntityExample>(entityId);

            if (!stateResponse.EntityExists)
            {
                await durableEntityClient.SignalEntityAsync<IEntityExample>(entityId, e => e.SetName(response.Name));
                // or
                context.SignalEntity<IEntityExample>(entityId, e => e.SetName(response.Name));

                context.StartNewOrchestration(nameof(OrchestrationExample), null);
            }
            return response.Name != null
                ? new OkObjectResult($"Hello, {response.Name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }

    public class MyClass
    {
        public string? Name { get; set; }
    }
}
