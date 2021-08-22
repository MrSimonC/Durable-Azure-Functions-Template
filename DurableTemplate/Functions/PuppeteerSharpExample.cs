using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace DurableTemplate.Functions
{
    public class PuppeteerSharpExample
    {
        private readonly AppInfo AppInfo;

        public PuppeteerSharpExample(AppInfo appInfo) => AppInfo = appInfo;

        [FunctionName("PuppeteerSharpExample")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [DurableClient] IDurableEntityClient client)
        {
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = AppInfo.BrowserExecutablePath
            });

            Page page = await browser.NewPageAsync();
            await page.SetUserAgentAsync("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36");

            string url = "https://www.bing.com";
            log.LogInformation($"going to url {url}");
            await page.GoToAsync(url);
            string? html = await page.GetContentAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string responseMessage = "response message";

            return new OkObjectResult(responseMessage);
        }
    }
}
