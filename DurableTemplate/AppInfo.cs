using DurableTemplate;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DurableTemplate;

public class AppInfo
{
    public string BrowserExecutablePath { get; set; } = string.Empty;
}
