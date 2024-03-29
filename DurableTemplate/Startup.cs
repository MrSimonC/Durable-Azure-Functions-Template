﻿using DurableTemplate;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;
using System.Runtime.InteropServices;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DurableTemplate;

public class Startup : FunctionsStartup
{
    public override async void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();

        //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();

        // for PupeteerSharp browser automation
        var bfOptions = new BrowserFetcherOptions();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            bfOptions.Path = Path.GetTempPath();
        }
        var bf = new BrowserFetcher(bfOptions);
        await bf.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        var info = new AppInfo
        {
            BrowserExecutablePath = bf.GetExecutablePath(BrowserFetcher.DefaultChromiumRevision)
        };
        builder.Services.AddSingleton(info);
    }
}
