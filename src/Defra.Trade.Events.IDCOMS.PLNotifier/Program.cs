// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Logging.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Defra.Trade.Events.IDCOMS.PLNotifier.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "No value in covering startup")]

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(configBuilder =>
    {
        try
        {
            configBuilder.ConfigureTradeAppConfiguration(config =>
            {
                config.UseKeyVaultSecrets = true;
                config.RefreshKeys.Add($"{PlNotifierSettings.PlNotifierSettingsName}:{PlNotifierSettings.AppConfigSentinelName}");
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Startup] Warning: Could not load Azure App Configuration: {ex.Message}");
        }
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services
            .AddTradeAppConfiguration(configuration)
            .AddServiceRegistrations(configuration)
            .AddApplication()
            .AddFunctionLogging("PLNotifier");

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName is string fullName && fullName.Contains("Defra"))
            .OrderBy(a => a.FullName)
            .ToList();
        services.AddAutoMapper(assembly);
        services.AddAutoMapper(typeof(Defra.Trade.Events.IDCOMS.PLNotifier.Application.Mappers.ApprovalProfile).Assembly);
    })
    .Build();

await host.RunAsync();
