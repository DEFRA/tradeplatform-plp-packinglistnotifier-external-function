// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Function.Health.DynamicsClient;
using Defra.Trade.Common.Functions;
using Defra.Trade.Common.Functions.EventStore;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Common.Functions.Services;
using Defra.Trade.Common.Functions.Validation;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Infrastructure;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Services;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Infrastructure;

[ExcludeFromCodeCoverage(Justification = "No value in covering startup")]
public static class ServiceRegistrations
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks();

        return services
            .AddValidators()
            .AddEventStore()
            .AddProcessor()
            .AddConfigurations(configuration)
            .AddDynamicsHealthCheckDependencies(configuration);
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ServiceBusQueuesSettings>().Bind(configuration.GetSection(ServiceBusSettings.OptionsName));
        var configName = configuration.GetSection(Models.PlNotifierSettings.PlNotifierSettingsName);
        services.AddOptions<Models.PlNotifierSettings>().Bind(configName);
        services.Configure<ServiceBusSettings>(configuration.GetSection(ServiceBusSettings.OptionsName));
        services.AddCrm(configuration.GetSection("PLP:Dynamics"));
        services.AddMessageRetryService();
        return services;
    }

    private static IServiceCollection AddDynamicsHealthCheckDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDynamicsClientAuthenticator, DynamicsClientAuthenticator>();

        var apiConfig = configuration.GetSection("PLP:Dynamics");
        services.AddOptions<DynamicsClientConfig>().Bind(apiConfig);
        return services;
    }

    private static IServiceCollection AddEventStore(this IServiceCollection services)
    {
        return services
            .AddEventStoreConfiguration()
            .AddSingleton<IMessageCollector, EventStoreCollector>();
    }

    private static IServiceCollection AddMessageRetryService(this IServiceCollection services)
    {
        return services
            .AddSingleton<MessageRetryService>()
            .AddSingleton<IMessageRetryService>(p => p.GetRequiredService<MessageRetryService>())
            .AddSingleton<IMessageRetryContextAccessor>(p => p.GetRequiredService<MessageRetryService>());
    }

    private static IServiceCollection AddProcessor(this IServiceCollection services)
    {
        return services
            .AddTransient<IMessageProcessor<Models.Approval, TradeEventMessageHeader>, ApprovalMessageProcessor>()
            .AddTransient<IBaseMessageProcessorService<Inbound.Approval>,
                BaseMessageProcessorService<Inbound.Approval, Models.Approval, Models.Approval, TradeEventMessageHeader>>();
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services
             .AddSingleton<ICustomValidatorFactory, CustomValidatorFactory>()
             .AddSingleton<AbstractValidator<TradeEventMessageHeader>, MessageHeaderValidator>()
             .AddSingleton<AbstractValidator<Inbound.Approval>, ApprovalValidator>()
             .AddTransient<IInboundMessageValidator<Inbound.Approval, TradeEventMessageHeader>,
                InboundMessageValidator<Inbound.Approval, Models.Approval, TradeEventMessageHeader>>();
    }
}
