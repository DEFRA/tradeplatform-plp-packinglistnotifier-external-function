// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Functions.EventStore;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Validation;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Infrastructure;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ServiceRegistrations
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddEventStore()
            .AddValidators()
            .AddConfigurations(configuration);
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ServiceBusQueuesSettings>().Bind(configuration.GetSection(ServiceBusSettings.OptionsName));
        var configName = configuration.GetSection(PlNotifierSettings.PlNotifierSettingsName);
        services.AddOptions<PlNotifierSettings>().Bind(configName);
        services.Configure<ServiceBusSettings>(configuration.GetSection(ServiceBusSettings.OptionsName));

        // TODO
        services.AddCrm(configuration.GetSection("TBC:Dynamics"));

        return services;
    }

    private static IServiceCollection AddEventStore(this IServiceCollection services)
    {
        return services
            .AddEventStoreConfiguration()
            .AddSingleton<IMessageCollector, EventStoreCollector>();
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICustomValidatorFactory, CustomValidatorFactory>();
    }
}
