// See https://aka.ms/new-console-template for more information

using KanjiReader;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Domain.WaniKani;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.ExternalServices.WaniKani;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register your services here
        services.AddHttpClient(); // Register IHttpClientFactory
        services.AddScoped<WaniKaniClient>();
        services.AddScoped<WatanocClient>();
        services.AddScoped<WaniKaniService>();
        services.AddScoped<TextProcessingService>();
    })
    .Build();
    
using var serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;

var myService = provider.GetRequiredService<TextProcessingService>();
await myService.ProcessText();

// SQL storage
// AI generated texts
// More text sources
// Kanji selection from dictionaty
// Account and authorization