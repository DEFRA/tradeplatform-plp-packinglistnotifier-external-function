// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Function.Health.HealthChecks;
using Defra.Trade.Common.Logging.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Defra.Trade.Events.IDCOMS.PLNotifier.Infrastructure;
using FunctionHealthCheck;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Defra.Trade.Events.IDCOMS.PLNotifier;

[ExcludeFromCodeCoverage(Justification = "No value in covering startup")]
public sealed class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        builder.Services
            .AddTradeAppConfiguration(configuration)
            .AddServiceRegistrations(configuration)
            .AddApplication()
            .AddFunctionLogging("PLNotifier");

        var healthChecksBuilder = builder.Services.AddFunctionHealthChecks();
        RegisterHealthChecks(healthChecksBuilder, builder.Services, configuration);

        builder.ConfigureMapper();
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
           .ConfigureTradeAppConfiguration(config =>
           {
               config.UseKeyVaultSecrets = true;
               config.RefreshKeys.Add($"{PlNotifierSettings.PlNotifierSettingsName}:{PlNotifierSettings.AppConfigSentinelName}");
           });
    }

    private static void RegisterHealthChecks(
        IHealthChecksBuilder builder,
        IServiceCollection services,
        IConfiguration configuration)
    {
        builder
            .AddCheck<AppSettingHealthCheck>("ServiceBus:ConnectionString")
            .AddCheck<AppSettingHealthCheck>("PLP:Dynamics:Authority")
            .AddCheck<AppSettingHealthCheck>("PLP:Dynamics:ClientId")
            .AddCheck<AppSettingHealthCheck>("PLP:Dynamics:ClientSecret")
            .AddCheck<AppSettingHealthCheck>("PLP:Dynamics:Resource");

        builder.AddAzureServiceBusCheck(
            configuration,
            "ServiceBus:ConnectionString",
            PlNotifierSettings.DefaultQueueName);

        builder.AddDynamicsCheck(services.BuildServiceProvider());
    }
}
