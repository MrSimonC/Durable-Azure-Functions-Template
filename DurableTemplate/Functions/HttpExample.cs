using DurableTemplate.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
            [DurableClient] IDurableEntityClient durableEntityClient)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // http client json
            string url = "https://mocki.io/v1/d569cadb-8f0b-4271-af06-84368787252d";
            MyClass? response = await httpClient.GetFromJsonAsync<MyClass>(url);

            // durable entity
            var entityId = new EntityId(nameof(EntityExample), "myEntity1");
            EntityStateResponse<EntityExample> stateResponse = await durableEntityClient.ReadEntityStateAsync<EntityExample>(entityId);
            await durableEntityClient.SignalEntityAsync<IEntityExample>(entityId, e => e.SetName(response.Name));

            if (!stateResponse.EntityExists)
            {
                string entityNameProperty = stateResponse.EntityState.Name;
                // ...
            }

            return new OkObjectResult($"Hello"); // or BadRequestObjectResult()
        }
    }

    public class MyClass
    {
        public string Name { get; set; } = string.Empty;
    }
}
