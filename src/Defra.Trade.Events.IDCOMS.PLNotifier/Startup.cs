// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Logging.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Defra.Trade.Events.IDCOMS.PLNotifier.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Defra.Trade.Events.IDCOMS.PLNotifier;

[ExcludeFromCodeCoverage]
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
}
